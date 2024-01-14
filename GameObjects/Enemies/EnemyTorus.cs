using Godot;
using System;

public class EnemyTorus : EnemyEntity
{
	private PathFollow2D _pathFollow = null;
	private EnemyProjectileShooter _enemyProjectileShooter;

	public override void _Ready()
	{
		if (GetParent().IsClass("PathFollow2D"))
		{
			_pathFollow = GetParent<PathFollow2D>();
		}
		_sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_enemyProjectileShooter = GetNode<EnemyProjectileShooter>("EnemyProjectileShooter");
	}

	public override void _PhysicsProcess(float delta)
	{
		if (!_enemyProjectileShooter.SeesPlayer())
		{
			_sprite.Play("Default");
			if (_pathFollow != null)
			{
				_pathFollow.Offset += speed * delta;
			}
			else
			{
				Fall(delta);
				velocity = MoveAndSlide(velocity, Vector2.Up);
			}
		}
		else
		{
			_sprite.Play("shoot");
		}
	}
}
