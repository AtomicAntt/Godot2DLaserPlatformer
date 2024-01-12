using Godot;
using System;

public class EnemyLaser : Node2D
{
    private Line2D _laser;
    private Position2D _endPos;
    private RayCast2D _rayCast;

    public int attackRange = 1000;

    [Export]
    public bool trackingEnabled = false;

    [Export]
    public int damage = 20;

    [Export]
    public int rotationSpeed = 50;

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

        if (trackingEnabled)
        {
            RotationDegrees += (rotationSpeed * delta);
            RotationDegrees = RotationDegrees % 360;
        }

        if (_rayCast.IsColliding())
        {
            _endPos.GlobalPosition = _rayCast.GetCollisionPoint();

            if (_rayCast.GetCollider().IsClass("KinematicBody2D"))
            {
                KinematicBody2D collider = (KinematicBody2D)_rayCast.GetCollider();
                if (collider.IsInGroup("player"))
                {
                    Player player = (Player)collider;
                    player.Hurt(damage);
                }
            }
        }
        else
        {
            // _endPos.GlobalPosition = _rayCast.GlobalPosition + new Vector2(_rayCast.CastTo.x, _rayCast.CastTo.y);
            _endPos.GlobalPosition = GlobalPosition + new Vector2(_rayCast.CastTo.x, _rayCast.CastTo.y);

        }

        _laser.SetPointPosition(1, _endPos.Position);
    }

    public void FlipLaser()
    {
        _rayCast.CastTo *= -1;
    }


}
