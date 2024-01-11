using Godot;
using System;

public class EnemyEntity : KinematicBody2D
{
    private PathFollow2D _pathFollow = null;
    private AnimatedSprite _sprite;

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
        GetParent().AddChild(particlesInstance);
        particlesInstance.GlobalPosition = GlobalPosition;
        QueueFree();
    }

    public void FlipRight(bool toggle)
    {
        _sprite.FlipH = toggle;
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_pathFollow != null)
        {
            _pathFollow.Offset += speed * delta;
        }
    }
}
