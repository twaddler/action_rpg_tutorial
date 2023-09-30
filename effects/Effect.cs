using Godot;

namespace action_rpg_tutorial_csharp.effects;

public partial class Effect : Node2D
{
	private AnimatedSprite2D _animSprite;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_animSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		_animSprite.Play("Animate");
		_animSprite.AnimationFinished += OnAnimationFinished;
	}

	private void OnAnimationFinished()
	{
		QueueFree();
	}
}
