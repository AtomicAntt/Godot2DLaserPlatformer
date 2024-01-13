using Godot;
using System;

public class EnemyEntity : KinematicBody2D
{
    private PathFollow2D _pathFollow = null;
    public AnimatedSprite _sprite;
    [Export]
    public Vector2 velocity = Vector2.Zero;

    public const int _Gravity = 1300;

    [Export]
    public int speed = 130;

    public override void _Ready()
    {
        if (GetParent().IsClass("PathFollow2D"))
        {
            _pathFollow = GetParent<PathFollow2D>();
        }
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public void Destruct()
    {
        PackedScene scene = GD.Load<PackedScene>("res://GameObjects/DeathParticles.tscn");
        DeathParticles particlesInstance = scene.Instance<DeathParticles>();
        Level level = GetTree().GetNodesInGroup("level")[0] as Level;
        level.AddChild(particlesInstance);
        particlesInstance.GlobalPosition = GlobalPosition;
        QueueFree();
    }

    public void FlipRight(bool toggle)
    {
        if (_sprite.FlipH != toggle)
        {
            _sprite.FlipH = toggle;
            foreach (Node2D node in GetChildren())
            {
                if (node.IsInGroup("enemyLaser"))
                {
                    EnemyLaser enemyLaser = node as EnemyLaser;
                    enemyLaser.FlipLaser();
                }
            }
        }
    }

    public void Fall(float delta)
    {
        velocity.y += _Gravity * delta;
    }

    public override void _PhysicsProcess(float delta)
    {
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
}
