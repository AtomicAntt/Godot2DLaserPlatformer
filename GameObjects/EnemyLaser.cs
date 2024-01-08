using Godot;
using System;

public class EnemyLaser : Node2D
{
    private Line2D _laser;
    private Position2D _endPos;
    private RayCast2D _rayCast;

    public override void _Ready()
    {
        _laser = GetNode<Line2D>("Line2D");
        _endPos = GetNode<Position2D>("Position2D");
        _rayCast = GetNode<RayCast2D>("RayCast2D");

        _laser.AddPoint(Vector2.Zero);
        _laser.AddPoint(Vector2.Zero);
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
            _endPos.GlobalPosition = _rayCast.GlobalPosition + new Vector2(_rayCast.CastTo.x, _rayCast.CastTo.y);
        }

        _laser.SetPointPosition(1, _endPos.Position);
    }


}
