using Godot;
using System;

public class Player : KinematicBody2D
{
    public enum States {AIR, FLOOR, DEAD, CHARGING, SHOOTING};

    [Export]
    public States state = States.AIR;
    [Export]
    public Vector2 velocity = Vector2.Zero;

    private Laser _laser;
    private AnimatedSprite _sprite;
    private Timer _sleepingTimer;
    private Timer _chargingTimer;

    private const int _Speed = 330;
    private const int _Gravity = 1300;
    private const int _Jump = -600;
    private const int _Laser_Radius = 80;
    private const int _Recoil = 400;

    
    public override void _Ready()
    {
        _laser = GetNode<Laser>("Laser");
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _sleepingTimer = GetNode<Timer>("SleepingTimer");
        _chargingTimer = GetNode<Timer>("ChargingTimer");
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

    public void FaceTowardsMouse()
    {
        if (GetGlobalMousePosition().x < GlobalPosition.x)
        {
            _sprite.FlipH = true;
        }
        else
        {
            _sprite.FlipH = false;
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

    // Called by manager laser shooting which is called by physics process
    public void toggleLaser(bool toggle)
    {
        // _shootingLaser = toggle;
        _laser.toggleLaserShooting(toggle);
    }

    public void ManageLaserShooting()
    {
        if (Input.IsActionJustPressed("leftclick"))
        {
            // _laser.Visible = true;
            // _laser.laserShooting = true;

            // toggleLaser(true);

            SetCharging();

        }
        else if (Input.IsActionJustReleased("leftclick"))
        {
            // _laser.laserShooting = false;
            // _laser.Visible = false;

            toggleLaser(false);
            _sprite.Play("Jump"); // Why fall? Because if you land on the ground or get enough velocity, the correct animation will play
            state = States.AIR; // Why air? Because if you are on the ground it will instantly set you on floor state.
        }
        else if (Input.IsActionPressed("leftclick") && state == States.SHOOTING)
        {
            toggleLaser(true);
        }

    }

    public void ManageDirection(string fireOrRecoil)
    {
        // GD.Print(GlobalPosition.DirectionTo(GetGlobalMousePosition()));
        double yDirectionTo = GlobalPosition.DirectionTo(GetGlobalMousePosition()).y;

        if (yDirectionTo >= -1 && yDirectionTo <= -0.6) 
        {
            _sprite.Play(fireOrRecoil + "Up");
        }
        else if (yDirectionTo > -0.6 && yDirectionTo <= -0.2)
        {
            _sprite.Play(fireOrRecoil + "DiagonalUp");
        }
        else if (yDirectionTo > -0.2 && yDirectionTo <= 0.2)
        {
            _sprite.Play(fireOrRecoil);
        }
        else if (yDirectionTo > 0.2 && yDirectionTo <= 0.6)
        {
            _sprite.Play(fireOrRecoil + "DiagonalDown");
        }
        else if (yDirectionTo > 0.6 && yDirectionTo <= 1)
        {
            _sprite.Play(fireOrRecoil + "Down");
        }

    }

    public void SetFalling()
    {
        _sprite.Play("Fall");
    }

    // Purpose: To allow for sleeping to eventually start if idling too long
    public void SetIdle()
    {
        _sprite.Play("Idle");
        _sleepingTimer.Start();
    }

    public void SetCharging()
    {
        state = States.CHARGING;
        _sprite.Play("Fire");
        _chargingTimer.Start();
    }

    public void SetShooting()
    {
        state = States.SHOOTING;
        _sprite.Play("Recoil");
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
                    SetIdle();
                }
                break;
            case States.FLOOR:
                // Purpose: If you have movement horizontally, it plays "Run" animation
                Fall(delta); // this may be fine?

                if (GetHorizontalMovement() && velocity.Abs().Length() > 100)
                {
                    _sprite.Play("Run");
                    _sleepingTimer.Stop(); // Cant go to sleep anymore

                }
                else if (_sprite.Animation == "Run")
                {
                    SetIdle();
                }

                if (Input.IsActionPressed("up"))
                {
                    velocity.y += _Jump;
                    state = States.AIR;
                    _sprite.Play("Jump");
                }
                else if (!IsOnFloor()){
                    state = States.AIR;
                    GD.Print("not on floor switching to air now");
                    // _sprite.Play("Fall");
                }

                break;
            case States.DEAD:
                break;
            case States.CHARGING:
                GetHorizontalMovement();
                FaceTowardsMouse(); // After horizontal movement to override the flip
                ManageDirection("Fire");
                Fall(delta); // this may be fine?
                break;
            case States.SHOOTING:
                GetHorizontalMovement();
                FaceTowardsMouse(); // After horizontal movement to override the flip
                ManageDirection("Recoil");
                Fall(delta); // this may be fine?
                velocity -= (_laser.GlobalPosition - GlobalPosition).Normalized() * _Recoil;
                break;

        }

        if (velocity.y > 50 && state != States.SHOOTING && state != States.CHARGING)
        {
            _sprite.Play("Fall");
        }

        // move this to switch statement later
        // if (state == States.SHOOTING)
        // {
        //     velocity -= (_laser.GlobalPosition - GlobalPosition).Normalized() * _Recoil;
        // }
    }

    // Purpose of both of these methods: Get player sleeping after some time, and wake up after the animation finishes.
    public void _on_SleepingTimer_timeout()
    {
        if (_sprite.Animation == "Idle")
        {
            _sprite.Play("Idle2");
        }
    }

    public void _on_AnimatedSprite_animation_finished()
    {
        if (_sprite.Animation == "Idle2")
        {
            SetIdle(); // So they will go back to sleeping again soon
        }
    }

    public void _on_ChargingTimer_timeout()
    {
        if (state == States.CHARGING)
        {
            SetShooting();
        }
        else
        {

        }
    }

}
