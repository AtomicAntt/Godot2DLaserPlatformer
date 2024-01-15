using Godot;
using System;

public class EnemyHumanoid : EnemyEntity
{
	private CollisionShape2D _collisionShape2D;
	private RayCast2D _floorChecker;
	private int direction = 1;

	[Export]
	public bool moving = true;

	private Vector2 _velocity = Vector2.Zero;

	public override void _Ready()
	{
		_sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_floorChecker = GetNode<RayCast2D>("FloorChecker");
		if (moving)
		{
			_sprite.Play("Default");
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (moving)
		{
			if (IsOnWall() || !_floorChecker.IsColliding() && IsOnFloor())
			{
				direction *= -1;
				_sprite.FlipH = !_sprite.FlipH;
			}

			Fall(delta);
			_velocity.y += _Gravity;
			_velocity.x = speed * direction;
			_velocity = MoveAndSlide(_velocity, Vector2.Up);
		}
	}
	
	public void setLaser(int speed, int damage){
		GetNode<EnemyLaser>("EnemyLaser").rotationSpeed = speed;
		GetNode<EnemyLaser>("EnemyLaser").damage = damage;
	}
}
