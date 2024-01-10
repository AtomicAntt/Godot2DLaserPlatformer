using Godot;
using System;

public class test : Area2D
{

    private int _speed = 50;
    private PathFollow2D _pathFollow = null;

    public override void _Ready()
    {
        _pathFollow = GetParent<PathFollow2D>();
    }
    public override void _PhysicsProcess(float delta)
    {
        _pathFollow.Offset += _pathFollow.Offset * _speed * delta;
    }
}
