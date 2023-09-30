using Godot;
using System;

public partial class PlayerHurtSound : AudioStreamPlayer2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Finished += OnFinished;
	}

	private void OnFinished()
	{
		QueueFree();
	}
}
