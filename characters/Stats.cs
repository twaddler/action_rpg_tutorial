using System;
using Godot;

namespace action_rpg_tutorial_csharp.characters;

public partial class Stats : Node
{
	[Export] private int _maxHealth = 1;
	public int MaxHealth
	{
		get => _maxHealth;
		private set
		{
			if (_maxHealth > 0)
			{
				_maxHealth = value;
				Health = Math.Min(Health, MaxHealth);
				EmitSignal(SignalName.MaxHealthChanged, value);
			}
			else
			{
				GD.PrintErr("Cannot set MaxHealth below 0;");
			}
		}
	}

	private int _health = 1;

	public int Health
	{
		get => _health;
		set
		{
			_health = value;
			EmitSignal(SignalName.HealthChanged, value);
			if (_health <= 0)
			{
				EmitSignal(SignalName.NoHealth);
			}
		}
	}

	public override void _Ready()
	{
		Health = MaxHealth;
	}

	[Signal]
	public delegate void NoHealthEventHandler();

	[Signal]
	public delegate void HealthChangedEventHandler(int newHealth);

	[Signal]
	public delegate void MaxHealthChangedEventHandler(int newMaxHealth);
}
