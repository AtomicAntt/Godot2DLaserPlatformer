using Godot;
using System;

public class Player : KinematicBody2D
{
    public enum States {AIR, FLOOR, DEAD};

    [Export]
    public States state = States.AIR;
    [Export]
    public Vector2 velocity = Vector2.Zero;

    private Laser _laser;

    private const int _Speed = 330;
    private const int _Gravity = 1300;
    private const int _Jump = -600;
    private const int _Laser_Radius = 30;
    private const int _Recoil = 400;

    private bool _shootingLaser = false;

    
    public override void _Ready()
    {
        _laser = GetNode<Laser>("Laser");
    }

    public void GetHorizontalMovement()
    {

        if (Input.IsActionPressed("right"))
        {
            velocity.x = Mathf.Lerp(velocity.x, _Speed, 0.1f);
        }
        else if (Input.IsActionPressed("left"))
        {
            velocity.x = Mathf.Lerp(velocity.x, -_Speed, 0.1f);
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, 0.5f);
        }
    }

    public void Fall(float delta)
    {
        velocity.y += _Gravity * delta;
    }

    public void moveLaserToMouse()
    {
        Vector2 mouseToDirection = (GetGlobalMousePosition() - GlobalPosition).Normalized();
        _laser.Position = (mouseToDirection * _Laser_Radius);
        _laser.LookAt(GlobalPosition);
    }

    public void manageLaserShooting()
    {
        if (Input.IsActionJustPressed("leftclick"))
        {
            // _laser.Visible = true;
            // _laser.laserShooting = true;
            _shootingLaser = true;
            _laser.toggleLaserShooting(true);
        }
        else if (Input.IsActionJustReleased("leftclick"))
        {
            // _laser.laserShooting = false;
            // _laser.Visible = false;
            _shootingLaser = false;
            _laser.toggleLaserShooting(false);
        }
    }

    public override void _PhysicsProcess(float delta)
    {
        velocity = MoveAndSlide(velocity, Vector2.Up);
        moveLaserToMouse();
        manageLaserShooting();
        switch(state)
        {
            case States.AIR:
                GetHorizontalMovement();
                Fall(delta);
                if (IsOnFloor())
                {
                    state = States.FLOOR;
                }
                break;
            case States.FLOOR:
                GetHorizontalMovement();
                if (Input.IsActionPressed("up"))
                {
                    velocity.y += _Jump;
                    state = States.AIR;
                }
                else if (!IsOnFloor()){
                    state = States.AIR;
                }

                break;
            case States.DEAD:
                break;

        }

        if (_shootingLaser)
        {
            velocity -= (_laser.GlobalPosition - GlobalPosition).Normalized() * _Recoil;
        }
    }


}
