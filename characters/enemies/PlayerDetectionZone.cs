using Godot;
using action_rpg_tutorial_csharp.characters.player;

namespace action_rpg_tutorial_csharp.characters.enemies;

public partial class PlayerDetectionZone : Area2D
{

	public Player Player { get; private set; }

	public override void _Ready()
	{
		GD.Print("Readying Detection");

		BodyEntered += OnPlayerEntered;
		BodyExited += OnPlayerExited;
	}

	private void OnPlayerEntered(Node2D body)
	{
		if (body is Player player)
		{
			Player = player;
		}
	}


	private void OnPlayerExited(Node2D body)
	{
		if (body is Player)
		{
			Player = null;
		}
	}

	public bool CanSeePlayer()
	{
		return Player != null;
	}

}
