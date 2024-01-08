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
    private AnimatedSprite _sprite;

    private const int _Speed = 330;
    private const int _Gravity = 1300;
    private const int _Jump = -600;
    private const int _Laser_Radius = 80;
    private const int _Recoil = 400;

    private bool _shootingLaser = false;

    
    public override void _Ready()
    {
        _laser = GetNode<Laser>("Laser");
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
    }

    public bool GetHorizontalMovement()
    {
        if (Input.IsActionPressed("right"))
        {
            velocity.x = Mathf.Lerp(velocity.x, _Speed, 0.1f);
            _sprite.FlipH = false;
            return true;
        }
        else if (Input.IsActionPressed("left"))
        {
            velocity.x = Mathf.Lerp(velocity.x, -_Speed, 0.1f);
            _sprite.FlipH = true;
            return true;
        }
        else
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, 0.5f);
            return false;
        }
    }

    public void Fall(float delta)
    {
        velocity.y += _Gravity * delta;
    }

    public void MoveLaserToMouse()
    {
        Vector2 mouseToDirection = (GetGlobalMousePosition() - GlobalPosition).Normalized();
        _laser.Position = (mouseToDirection * _Laser_Radius);
        _laser.LookAt(GlobalPosition);
    }

    public void ManageLaserShooting()
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

    public void SetFalling()
    {
        _sprite.Play("Fall");
    }

    public override void _PhysicsProcess(float delta)
    {
        velocity = MoveAndSlide(velocity, Vector2.Up);
        MoveLaserToMouse();
        ManageLaserShooting();
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
                // Purpose: If you have movement horizontally, it plays "Run" animation
                if (GetHorizontalMovement() && velocity.Abs().Length() > 100)
                {
                    _sprite.Play("Run");
                }
                else
                {
                    _sprite.Play("Idle");
                }

                if (Input.IsActionPressed("up"))
                {
                    velocity.y += _Jump;
                    state = States.AIR;
                    _sprite.Play("Jump");
                }
                else if (!IsOnFloor()){
                    state = States.AIR;
                    // _sprite.Play("Fall");
                }

                break;
            case States.DEAD:
                break;

        }

        if (velocity.y > 50) // So going down
        {
            _sprite.Play("Fall");
        }

        if (_shootingLaser)
        {
            velocity -= (_laser.GlobalPosition - GlobalPosition).Normalized() * _Recoil;
        }
    }

}
