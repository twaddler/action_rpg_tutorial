using System;
using Godot;

namespace action_rpg_tutorial_csharp.characters.enemies;

public partial class WanderController : Node2D
{
	[Export] private int _wanderRange = 32;
	
	private Timer _timer;

	private Vector2 _startPosition;
	public Vector2 TargetPosition { get; private set; }
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_startPosition = GlobalPosition;
		TargetPosition = GlobalPosition;
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += OnTimeout;
	}

	private void OnTimeout()
	{
		UpdateTargetPosition();
	}

	private void UpdateTargetPosition()
	{
		var rand = new Random();
		var targetVector = new Vector2(rand.Next(-_wanderRange, _wanderRange), rand.Next(-_wanderRange, _wanderRange));
		TargetPosition = _startPosition + targetVector;
	}

	public double GetTimeLeft()
	{
		return _timer.TimeLeft;
	}

	public void StartWanderTimer(double duration)
	{
		_timer.Start(duration);
	}
}