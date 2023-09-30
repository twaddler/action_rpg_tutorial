using Godot;
using System;
using action_rpg_tutorial_csharp.characters;

namespace action_rpg_tutorial_csharp.UI;

public partial class HealthUI : Control
{
	private int _hearts;

	private int Hearts
	{
		get => _hearts;
		set
		{
			_hearts = value;
			SetHeartWidth(_fullHearts, _hearts);
		}
	}

	private int _maxHearts;
	private int MaxHearts
	{
		get => _maxHearts;
		set
		{
			_maxHearts = value;
			SetHeartWidth(_emptyHearts, _maxHearts);
			if (Hearts > _maxHearts)
			{
				Hearts = _maxHearts;
			}
		}
	}

	private Stats _stats;
	private TextureRect _emptyHearts;
	private TextureRect _fullHearts;

	private float _heartPixelSize = 15;

	public override void _Ready()
	{
		_stats = GetNode<Stats>("/root/PlayerStats");
		_emptyHearts = GetNode<TextureRect>("HeartUIEmpty");
		_fullHearts = GetNode<TextureRect>("HeartUIFull");

		_stats.HealthChanged += OnHealthChanged;
		_stats.MaxHealthChanged += OnMaxHealthChanged;

		MaxHearts = _stats.MaxHealth;
		Hearts = MaxHearts;
		
		SetHeartWidth(_emptyHearts, MaxHearts);
		SetHeartWidth(_fullHearts, Hearts);
	}

	private void OnMaxHealthChanged(int newMaxHealth)
	{
		MaxHearts = newMaxHealth;
	}

	private void OnHealthChanged(int newHealth)
	{
		Hearts = newHealth;
	}

	private void SetHeartWidth(TextureRect heartTexture, int health)
	{
		var size = heartTexture.Size; 
		size.X = _heartPixelSize * health;
		if (size.X > 0)
		{
			heartTexture.Visible = true;
			heartTexture.Size = size;
		}
		else
		{
			heartTexture.Visible = false;
		}
	}
}
