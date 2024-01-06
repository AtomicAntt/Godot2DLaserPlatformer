using Godot;
using System;

public class Laser : Node2D
{
    private RayCast2D _rayCast;
    private Line2D _laser;
    private Position2D _endPos;

    public override void _Ready()
    {
        _rayCast = GetNode<RayCast2D>("RayCast2D");
        _laser = GetNode<Line2D>("Line2D");
        _endPos = GetNode<Position2D>("Position2D");

        _laser.AddPoint(Vector2.Zero);
        _laser.AddPoint(_rayCast.CastTo);
    }

    public override void _PhysicsProcess(float delta)
    {
        _laser.SetPointPosition(0, Vector2.Zero);
        if (_rayCast.IsColliding())
        {
            _endPos.GlobalPosition = _rayCast.GetCollisionPoint();
        }
        else
        {
            _endPos.GlobalPosition = _rayCast.CastTo;
        }
        _laser.SetPointPosition(1, _endPos.Position);
    }
}
