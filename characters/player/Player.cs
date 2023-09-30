using Godot;
using System;
using action_rpg_tutorial_csharp.hitboxes_hurtboxes;

namespace action_rpg_tutorial_csharp.characters.player;

public partial class Player : CharacterBody2D
{
	[Export] public float MaxSpeed = 100.0f;

	[Export] public float Acceleration = 10;

	[Export] public float Friction = 10;

	[Export] public float RollSpeed = 100;

	[Export] private float _invincibilityTimer = 0.6f;

	private AnimationPlayer _animPlayer;
	private AnimationPlayer _blinkAnimPlayer;
	private AnimationTree _animTree;

	private AnimationNodeStateMachinePlayback _animNode;

	private SwordHitbox _swordHitbox;
	private CollisionShape2D _swordShape;

	private PackedScene _deathSoundScene =
		ResourceLoader.Load<PackedScene>("res://characters/player/player_hurt_sound.tscn");

	private enum PlayerStates
	{
		Move,
		Attack,
		Roll
	}

	private PlayerStates _currentState = PlayerStates.Move;

	private Vector2 _direction;
	private Vector2 _rollDirection = Vector2.Down;
	private bool _hasAttacked;
	private bool _hasRolled;

	private Stats _playerStats;
	private Hurtbox _hurtbox;

	public override void _Ready()
	{
		_playerStats = GetNode<Stats>("/root/PlayerStats");
		_playerStats.NoHealth += OnPlayerNoHealth;

		_hurtbox = GetNode<Hurtbox>("Hurtbox");
		_hurtbox.AreaEntered += OnPlayerHurtBoxEntered;
		_hurtbox.InvincibilityStarted += OnInvincibilityStarted;
		_hurtbox.InvincibilityEnded += OnInvincibilityEnded;
		
		_animPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		_blinkAnimPlayer = GetNode<AnimationPlayer>("BlinkAnimationPlayer");
		_animTree = GetNode<AnimationTree>("AnimationTree");
		_swordHitbox = GetNode<SwordHitbox>("SwordHitbox");
		_swordShape = _swordHitbox.GetNode<CollisionShape2D>("CollisionShape2D");
		_swordHitbox.Monitoring = false;
		_swordShape.Disabled = true;
		_animTree.Active = true;
		_animTree.AnimationFinished += OnAnimationTreeAnimationFinished;

		_animNode = (AnimationNodeStateMachinePlayback) _animTree.Get("parameters/playback");
	}

	private void OnInvincibilityEnded()
	{
		_blinkAnimPlayer.Play("Stop");
	}

	private void OnInvincibilityStarted()
	{
		_blinkAnimPlayer.Play("Start");
	}

	private void OnPlayerHurtBoxEntered(Area2D area)
	{
		if (area is Hitbox hitbox && !_hurtbox.Invincible)
		{
			_hurtbox.StartInvincibility(_invincibilityTimer);
			_hurtbox.CreateHitEffect();
			_playerStats.Health -= hitbox.Damage;
		}
	}

	private void OnPlayerNoHealth()
	{
		var hurtSound = _deathSoundScene.Instantiate<PlayerHurtSound>();
		GetParent().AddChild(hurtSound);
		QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionPressed("attack") && !_hasAttacked)
		{
			_hasAttacked = true;
			_currentState = PlayerStates.Attack;
			Velocity = Vector2.Zero;
			_swordHitbox.Monitoring = true;
			_swordShape.Disabled = false;
		}

		if (Input.IsActionPressed("roll") && !_hasRolled)
		{
			_hasRolled = true;
			_currentState = PlayerStates.Roll;
		}

		switch (_currentState)
		{
			case PlayerStates.Move:
				Move(delta);
				break;
			case PlayerStates.Attack:
				Attack(delta);
				break;
			case PlayerStates.Roll:
				Roll(delta);
				break;
			default:
				throw new ArgumentOutOfRangeException();
		}
	}

	private void Move(double delta)
	{
		var velocity = Velocity;

		_direction = Input.GetVector("left", "right", "up", "down").Normalized();
		if (_direction != Vector2.Zero)
		{
			_rollDirection = _direction;
			_swordHitbox.KnockbackVector = _direction;
			_animTree.Set("parameters/Idle/blend_position", _direction);
			_animTree.Set("parameters/Run/blend_position", _direction);
			_animTree.Set("parameters/Attack/blend_position", _direction);
			_animTree.Set("parameters/Roll/blend_position", _direction);
			velocity = velocity.MoveToward(_direction * MaxSpeed, Acceleration * (float) delta);
			_animNode.Travel("Run");
		}
		else
		{
			velocity = velocity.MoveToward(Vector2.Zero, Friction * (float) delta);
			_animNode.Travel("Idle");
		}


		Velocity = velocity.LimitLength(MaxSpeed);
		MoveAndSlide();
	}

	private void Attack(double delta)
	{
		_swordHitbox.KnockbackVector = _rollDirection;
		_animNode.Travel("Attack");
	}

	private void Roll(double delta)
	{
		var velocity = _rollDirection * RollSpeed;
		_animNode.Travel("Roll");
		Velocity = velocity;
		MoveAndSlide();
	}

	private void OnAnimationTreeAnimationFinished(StringName animName)
	{
		if (animName == "attack_down" || animName == "attack_up" || animName == "attack_left" ||
		    animName == "attack_right")
		{
			_currentState = PlayerStates.Move;
			_hasAttacked = false;
			_swordHitbox.Monitoring = false;
			_swordShape.Disabled = true;
		}
		else if (animName == "roll_down" || animName == "roll_up" || animName == "roll_left" ||
		         animName == "roll_right")
		{
			_currentState = PlayerStates.Move;
			_hasRolled = false;
		}
	}
}
