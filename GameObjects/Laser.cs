using Godot;
using System;

public class Laser : Node2D
{
    [Export]
    public float laserWidth = 100.0f;

    // private RayCast2D _rayCast;
    private Node2D _rayCasts;
    private Line2D _laser;
    private Position2D _endPos;
    // private Player _player;

    private bool _laserShooting = false;

    public override void _Ready()
    {
        // _rayCast = GetNode<RayCast2D>("RayCast2D");
        _laser = GetNode<Line2D>("Line2D");
        _endPos = GetNode<Position2D>("Position2D");
        _rayCasts = GetNode<Node2D>("RayCasts");
        // _player = GetParent() as Player;

        _laser.AddPoint(Vector2.Zero);
        _laser.AddPoint(Vector2.Zero);
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
            // bool colliding = false;

            _laser.SetPointPosition(0, Vector2.Zero);

            foreach (RayCast2D rayCast in _rayCasts.GetChildren())
            {
                if (rayCast.IsColliding())
                {
                    // colliding = true;
                    // _endPos.GlobalPosition = rayCast.GetCollisionPoint();
                    // Purpose of normalized vector being added: Need to go deeper to register as in that tile, not just the one right outside it.
                    destroyTiles(rayCast.GetCollisionPoint() + (rayCast.GetCollisionPoint() - GlobalPosition).Normalized());
                }
            }
            // if (!colliding)
            // {
                // _endPos.GlobalPosition = _rayCast.CastTo;
            _endPos.GlobalPosition = GlobalPosition + (GetGlobalMousePosition() - GlobalPosition).Normalized() * 2000;
            // }

            _laser.SetPointPosition(1, _endPos.Position);
        }
    }
}
