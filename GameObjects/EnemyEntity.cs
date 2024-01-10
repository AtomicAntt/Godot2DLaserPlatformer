using Godot;
using System;

public class EnemyEntity : KinematicBody2D
{
    private PathFollow2D _pathFollow = null;

    private int _speed = 130;

    public override void _Ready()
    {
        if (GetParent().IsClass("PathFollow2D"))
        {
            _pathFollow = GetParent<PathFollow2D>();
        }
    }

    public void Destruct()
    {
        PackedScene scene = GD.Load<PackedScene>("res://GameObjects/DeathParticles.tscn");
        DeathParticles particlesInstance = scene.Instance<DeathParticles>();
        GetParent().AddChild(particlesInstance);
        particlesInstance.GlobalPosition = GlobalPosition;
        QueueFree();
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_pathFollow != null)
        {
            _pathFollow.Offset += _speed * delta;
        }
    }
}
