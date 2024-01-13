using Godot;
using System;

public class Laser : Node2D
{
    [Export]
    public float laserWidth = 96.0f;

    // private RayCast2D _rayCast;
    private Node2D _rayCasts;
    private Line2D _laser;
    private Position2D _endPos;
    private Sprite _startLaser;
    private Player _player;

    public bool laserShooting = false;

    public override void _Ready()
    {
        // _rayCast = GetNode<RayCast2D>("RayCast2D");
        _laser = GetNode<Line2D>("Line2D");
        _endPos = GetNode<Position2D>("Position2D");
        _rayCasts = GetNode<Node2D>("RayCasts");
        _startLaser = GetNode<Sprite>("StartLaser");
        _player = GetParent() as Player;

        _laser.AddPoint(Vector2.Zero);
        _laser.AddPoint(Vector2.Zero);
    }

    // player calls this
    public void toggleLaserShooting(bool isShooting)
    {
        SceneTreeTween tween = GetTree().CreateTween();
        if (isShooting)
        {
            startLaser();
            tween.TweenProperty(_laser, "width", laserWidth, 0.1f);
            tween.Parallel().TweenProperty(_startLaser, "scale", new Vector2(1.0f, _startLaser.Scale.y), 0.1f);
        }
        else
        {
            stopLaser();
            tween.TweenProperty(_laser, "width", 0.0f, 0.2f);
            tween.Parallel().TweenProperty(_startLaser, "scale", new Vector2(0.0f, _startLaser.Scale.y), 0.2f);
            // tween.Connect("finished", this, "stopLaser");

        }
    }

    public void stopLaser()
    {

        // _startLaser.Visible = false;
        // Visible = false;
        laserShooting = false;
    }

    public void startLaser()
    {
        _startLaser.Visible = true;
        Visible = true;
        laserShooting = true;
    }

    public void destroyTiles(Vector2 destroyPos)
    {
        Level levelNode = GetTree().GetNodesInGroup("level")[0] as Level;
        levelNode.destroyTile(destroyPos);
    }

    public override void _PhysicsProcess(float delta)
    {
        if (laserShooting)
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

                    destroyTiles(rayCast.GetCollisionPoint()); // Purpose: Lets doubly make sure that tile is destroyed
                    // GD.Print(rayCast.GetCollider().GetClass());

                    // First, is it an enemy?
                    if (rayCast.GetCollider().IsClass("KinematicBody2D"))
                    {
                        KinematicBody2D collider = (KinematicBody2D)rayCast.GetCollider();
                        if (collider.IsInGroup("enemy"))
                        {
                            EnemyEntity enemy = (EnemyEntity)collider;
                            enemy.Destruct();
                            break; // purpose: only need to destruct the enemy one time
                        }
                    }

                    // Now, is it a keycard or computer?
                    if (rayCast.GetCollider().IsClass("Area2D"))
                    {
                        Area2D collider = (Area2D)rayCast.GetCollider();
                        if (collider.IsInGroup("keycard"))
                        {
                            Keycard keycard = (Keycard)collider;
                            keycard.Destroy();
                            break;
                        }
                        else if (collider.IsInGroup("computer"))
                        {
                            Computer computer = (Computer)collider;
                            computer.Destroy();
                            break;
                        }
                    }
                }
            }
            // if (!colliding)
            // {
                // _endPos.GlobalPosition = _rayCast.CastTo;

            // _endPos.GlobalPosition = GlobalPosition + (GetGlobalMousePosition() - GlobalPosition).Normalized() * 2000;
            _endPos.GlobalPosition = GlobalPosition + (GlobalPosition - _player.GlobalPosition).Normalized() * 2000;

            // }

            _laser.SetPointPosition(1, _endPos.Position);
        }
    }
}
