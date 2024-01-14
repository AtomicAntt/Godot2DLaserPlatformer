using Godot;
using System;

public class EnemyProjectile : Area2D
{
	private Vector2 _direction;
	private AnimatedSprite _animatedSprite;

	private Player _player;

	[Export]
	public int speed = 200;

	[Export]
	public int damage = 10;


	public override void _Ready()
	{
		_animatedSprite = GetNode<AnimatedSprite>("AnimatedSprite");

		_player = GetTree().GetNodesInGroup("player")[0] as Player;

		if (IsInstanceValid(_player))
		{
			_direction = (_player.GlobalPosition - GlobalPosition).Normalized();
		}
	}

	public void _on_EnemyProjectile_body_entered(Node2D body)
	{
		_animatedSprite.Play("explode");

		if (body.IsInGroup("player"))
		{
			_player.Hurt(damage);
		}
	}


	// Purpose: Get rid of the node after the explosion animation
	public void _on_AnimatedSprite_animation_finished()
	{
		if (_animatedSprite.Animation == "explode")
		{
			QueueFree();
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (_animatedSprite.Animation != "explode")
		{
			Position += _direction * speed * delta;
		}
	}
}
