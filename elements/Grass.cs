using Godot;
using action_rpg_tutorial_csharp.characters.player;
using action_rpg_tutorial_csharp.effects;

namespace action_rpg_tutorial_csharp.elements;
public partial class Grass : Node2D
{
	private Area2D _hurtbox;
	private PackedScene _grassEffectScene = ResourceLoader.Load<PackedScene>("res://effects/grass_effect.tscn");

	public override void _Ready()
	{
		_hurtbox = GetNode<Area2D>("Hurtbox");
		_hurtbox.AreaEntered += OnAreaEntered;
	}

	private void OnAreaEntered(Area2D area)
	{
		if (area is SwordHitbox)
		{
			CreateGrassEffect();
			QueueFree();
		}
	}

	private void CreateGrassEffect()
	{
		var grassEffect = _grassEffectScene.Instantiate<Effect>();
		grassEffect.Position = Position;
		GetParent().AddChild(grassEffect);
	}
}
