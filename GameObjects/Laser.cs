using Godot;
using System;

public class Laser : Node2D
{
    [Export]
    public float laserWidth = 100.0f;

    private RayCast2D _rayCast;
    private Line2D _laser;
    private Position2D _endPos;

    private bool _laserShooting = false;

    public override void _Ready()
    {
        _rayCast = GetNode<RayCast2D>("RayCast2D");
        _laser = GetNode<Line2D>("Line2D");
        _endPos = GetNode<Position2D>("Position2D");

        _laser.AddPoint(Vector2.Zero);
        _laser.AddPoint(_rayCast.CastTo);
    }

    public void toggleLaserShooting(bool isShooting)
    {
        SceneTreeTween tween = GetTree().CreateTween();
        if (isShooting)
        {
            Visible = true;
            _laserShooting = true;
            tween.TweenProperty(_laser, "width", laserWidth, 0.1f);
        }
        else
        {
            tween.TweenProperty(_laser, "width", 0.0f, 0.2f);
            tween.Connect("finished", this, "stopLaser");

        }
    }

    public void stopLaser()
    {
        GD.Print("Laser has been stopped");
        Visible = false;
        _laserShooting = false;
    }

    public void destroyTiles(Vector2 destroyPos)
    {
        Level levelNode = GetTree().GetNodesInGroup("level")[0] as Level;
        levelNode.destroyTile(destroyPos);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_laserShooting)
        {
            _laser.SetPointPosition(0, Vector2.Zero);
            if (_rayCast.IsColliding())
            {
                _endPos.GlobalPosition = _rayCast.GetCollisionPoint();
                destroyTiles(_rayCast.GetCollisionPoint());
            }
            else
            {
                _endPos.GlobalPosition = _rayCast.CastTo;
            }
         _laser.SetPointPosition(1, _endPos.Position);
        }
    }
}
