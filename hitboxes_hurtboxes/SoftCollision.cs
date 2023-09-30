using Godot;
using System;

namespace action_rpg_tutorial_csharp.hitboxes_hurtboxes;
public partial class SoftCollision : Area2D
{
	public bool IsColliding()
	{
		return GetOverlappingAreas().Count > 0;
	}

	public Vector2 GetPushVector()
	{
		var pushVector = Vector2.Zero;
		if (IsColliding())
		{
			var collider = GetOverlappingAreas()[0];
			pushVector = collider.GlobalPosition.DirectionTo(GlobalPosition);
		}

		return pushVector;
	}
}

