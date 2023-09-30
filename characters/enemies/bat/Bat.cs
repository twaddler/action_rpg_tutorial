using System;
using System.Collections.Generic;
using Godot;
using action_rpg_tutorial_csharp.characters.player;
using action_rpg_tutorial_csharp.effects;
using action_rpg_tutorial_csharp.hitboxes_hurtboxes;

namespace action_rpg_tutorial_csharp.characters.enemies.bat;
public partial class Bat : CharacterBody2D
{
	private AnimatedSprite2D _animPlayer;
	private Hurtbox _hurtbox;
	private Stats _stats;
	private PackedScene _deathEffectScene = ResourceLoader.Load<PackedScene>("res://effects/bat_death_effect.tscn");
	private PlayerDetectionZone _playerDetectionZone;
	private SoftCollision _softCollision;
	private WanderController _wanderController;
	private AnimationPlayer _blinkAnimPlayer;

	private Vector2 _knockback = Vector2.Zero;
	private Vector2 _velocity = Vector2.Zero;

	private Player _player;

	[Export] private float _knockbackSpeed = 50;
	[Export] private float _friction = 100;
	[Export] private float _maxSpeed = 200;
	[Export] private float _maxWanderDistance = 5; 
	[Export] private float _acceleration = 100;
	[Export] private float _pushSpeed = 400;
	[Export] private float _invincibilityDuration = 0.3f;

	private enum BatStates
	{
		Idle,
		Wander,
		Chase
	}

	private BatStates _currentState = BatStates.Idle;

	public override void _Ready()
	{
		_animPlayer = GetNode<AnimatedSprite2D>("Body");
		_hurtbox = GetNode<Hurtbox>("Hurtbox");
		_stats = GetNode<Stats>("Stats");
		_playerDetectionZone = GetNode<PlayerDetectionZone>("PlayerDetectionZone");
		_softCollision = GetNode<SoftCollision>("SoftCollision");
		_wanderController = GetNode<WanderController>("WanderController");
		_blinkAnimPlayer = GetNode<AnimationPlayer>("BlinkAnimationPlayer");
		_stats.NoHealth += OnNoHealth;

		_hurtbox.AreaEntered += OnAreaEntered;
		_hurtbox.InvincibilityStarted += OnInvincibilityStarted;
		_hurtbox.InvincibilityEnded += OnInvincibilityEnded;
		_animPlayer.Play("fly");
	}
	private void OnInvincibilityEnded()
	{
		_blinkAnimPlayer.Play("Stop");
	}

	private void OnInvincibilityStarted()
	{
		_blinkAnimPlayer.Play("Start");
	}
	

	private void OnNoHealth()
	{
		QueueFree();
		var deathEffect = _deathEffectScene.Instantiate<Effect>();
		deathEffect.Position = Position;
		GetParent().AddChild(deathEffect);
	}

	public override void _PhysicsProcess(double delta)
	{
		_knockback = _knockback.MoveToward(Vector2.Zero, _friction * (float) delta);
		if (_knockback != Vector2.Zero)
		{
			Velocity = _knockback;
			MoveAndSlide();
		}

		switch (_currentState)
		{
			case BatStates.Idle:
				Idle(delta);
				break;
			case BatStates.Wander:
				Wander(delta);
				break;
			case BatStates.Chase:
				Chase(delta);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}

		if (_softCollision.IsColliding())
		{
			Velocity += _softCollision.GetPushVector() * (float)delta * _pushSpeed;
		}
		MoveAndSlide();
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is not SwordHitbox hitbox) return;
		if (_hurtbox.Invincible) return;
		_stats.Health -= hitbox.Damage;
		_knockback = hitbox.KnockbackVector * _knockbackSpeed;
		_hurtbox.CreateHitEffect();
		_hurtbox.StartInvincibility(_invincibilityDuration);
	}

	private void Idle(double delta)
	{
		Velocity = Velocity.MoveToward(Vector2.Zero, _friction * (float) delta);
		SeekPlayer();

		if (_wanderController.GetTimeLeft() != 0) return;
		UpdateWander();
		_wanderController.StartWanderTimer(new Random().Next(1, 4));
	}

	private void Wander(double delta)
	{
		SeekPlayer();

		if (_wanderController.GetTimeLeft() == 0)
		{
			UpdateWander();
			_wanderController.StartWanderTimer(new Random().Next(1, 4));
		}
		
		AccelerateTowardsPoint(_wanderController.TargetPosition, delta);

		if (GlobalPosition.DistanceTo(_wanderController.TargetPosition) <= _maxWanderDistance)
		{
			UpdateWander();
		}
	}

	private void Chase(double delta)
	{
		_player = _playerDetectionZone.Player;
		if (_player != null)
		{
			AccelerateTowardsPoint(_player.GlobalPosition, delta);
		}
		else
		{
			_currentState = BatStates.Idle;
		}


	}

	private void UpdateWander()
	{
		var randomStates = new List<BatStates>
		{
			BatStates.Idle,
			BatStates.Wander
		};
		_currentState = GetRandomState(randomStates);
	}

	private void AccelerateTowardsPoint(Vector2 position, double delta)
	{
		Vector2 direction = GlobalPosition.DirectionTo(position);
		Velocity = Velocity.MoveToward(direction * _maxSpeed, _acceleration * (float) delta);
		_animPlayer.FlipH = Velocity.X < 0;
	}

	private void SeekPlayer()
	{
		if (_playerDetectionZone.CanSeePlayer())
		{
			_currentState = BatStates.Chase;
		}
	}

	private BatStates GetRandomState(List<BatStates> states)
	{
		var rand = new Random();
		var randInt = rand.Next(0, states.Count);
		return states[randInt];
	}
}

