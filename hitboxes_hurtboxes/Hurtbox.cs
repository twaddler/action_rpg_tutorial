using action_rpg_tutorial_csharp.effects;
using Godot;

namespace action_rpg_tutorial_csharp.hitboxes_hurtboxes;

public partial class Hurtbox : Area2D
{
	private PackedScene _hitEffectScene = ResourceLoader.Load<PackedScene>("res://effects/bat_hit_effect.tscn");
	private CollisionShape2D _collisionShape;
	private Timer _timer;

	private bool _invincible;
	public bool Invincible
	{
		set
		{
			_invincible = value;
			EmitSignal(_invincible ? SignalName.InvincibilityStarted : SignalName.InvincibilityEnded);
		}
		get => _invincible;
	}

	[Signal]
	public delegate void InvincibilityStartedEventHandler();

	[Signal]
	public delegate void InvincibilityEndedEventHandler();

	public override void _Ready()
	{
		_timer = GetNode<Timer>("InvincibilityTimer");
		_collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		_timer.Timeout += OnInvincibilityTimeout;
	}

	private void OnInvincibilityTimeout()
	{
		Invincible = false;
	}

	public void CreateHitEffect()
	{
		var hitEffect = _hitEffectScene.Instantiate<Effect>();
		var mainScene = GetTree().CurrentScene;
		mainScene.AddChild(hitEffect);
		hitEffect.GlobalPosition = GlobalPosition;
	}

	public void StartInvincibility(double duration)
	{
		Invincible = true;
		_timer.Start(duration);
	}
}
