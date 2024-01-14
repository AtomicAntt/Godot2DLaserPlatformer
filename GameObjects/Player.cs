using Godot;
using System;

public class Player : KinematicBody2D
{
    public enum States {AIR, FLOOR, DEAD, CHARGING, SHOOTING, RECOVER, INACTIVE};

    [Signal]
    delegate void ShotLaser();

    [Export]
    public States state = States.AIR;
    [Export]
    public bool immune = false;
    [Export]
    public Vector2 velocity = Vector2.Zero;


    [Export]
    public int totalHealth = 100;
    [Export]
    public int health = 100;

    [Export]
    public bool hasKeycard = false;

    private Laser _laser;
    private AnimatedSprite _sprite;
    private Timer _sleepingTimer;
    private Timer _chargingTimer;
    private AnimationPlayer _animationPlayer;

    private const int _Speed = 330;
    private const int _Gravity = 1300;
    private const int _Jump = -600;
    private const int _Laser_Radius = 80;
    private const int _Recoil = 400;

    private double _soFarLaserDamage = 0.0; // Purpose: If this reaches >= 0.25, it resets and does 1 damage. It is incremented with delta.

    
    public override void _Ready()
    {
        _laser = GetNode<Laser>("Laser");
        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _sleepingTimer = GetNode<Timer>("SleepingTimer");
        _chargingTimer = GetNode<Timer>("ChargingTimer");
        _animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

        HealthBar healthBar = GetTree().GetNodesInGroup("healthBar")[0] as HealthBar;

        healthBar.ResetHealth();
    }

    public void DisableMovement()
    {
        state = States.INACTIVE;
    }

    public void EnableMovement()
    {
        state = States.AIR;
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

    // Remember, these 2 functions are called ONLY by animationplayer to handle immunity during hurt animation
    public void SetImmune()
    {
        immune = true;
    }

    public void RemoveImmunity()
    {
        immune = false;
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
        EmitSignal("ShotLaser"); // Purpose: For tutorial level
        state = States.SHOOTING;
        _sprite.Play("Recoil");
        Hurt(1); // Every time you start the laser, you take at least one damage
    }

    public void Die()
    {
        GD.Print("game over");
        state = States.DEAD;
        _sprite.Play("Dead");
        toggleLaser(false);
    }

    public void Hurt(int damageTaken)
    {
        // or statement is here because laser can hurt you too
        if ((!immune && state != States.DEAD) || (state == States.SHOOTING && damageTaken == 1))
        {
            health -= damageTaken;
            // GD.Print("player hurt, health left: " + health + "/" + totalHealth);

            // Here we are assuming there that the first (and only) node in group healthBar is actually a health bar
            HealthBar healthBar = GetTree().GetNodesInGroup("healthBar")[0] as HealthBar;

            healthBar.TakeDamage(damageTaken);


            if (health <= 0)
            {
                Die();
            }
            else if (!(state == States.SHOOTING && damageTaken == 1)) // no free immunity for using laser
            {
                _animationPlayer.Play("Hurt");
            }
        }

    }

    public void Recover()
    {
        state = States.RECOVER;
        _sprite.Play("Recover");
        HealthBar healthBar = GetTree().GetNodesInGroup("healthBar")[0] as HealthBar;

        healthBar.ResetHealth();
        health = totalHealth;
    }

    

    public override void _PhysicsProcess(float delta)
    {
        velocity = MoveAndSlide(velocity, Vector2.Up);
        if (state != States.DEAD && state != States.INACTIVE)
        {
            MoveLaserToMouse();
            ManageLaserShooting();
        }
        
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
                    // _sprite.Play("Fall");
                }

                break;
            case States.DEAD:
                Fall(delta);
                velocity.x = Mathf.Lerp(velocity.x, 0, 0.5f);
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

                _soFarLaserDamage += delta;
                if (_soFarLaserDamage >= 0.2){
                    _soFarLaserDamage -= 0.2;
                    Hurt(1);
                }


                break;
            case States.RECOVER:
                Fall(delta);
                velocity.x = Mathf.Lerp(velocity.x, 0, 0.5f);
                break;
            case States.INACTIVE:
                break;

        }

        if (velocity.y > 50 && state != States.SHOOTING && state != States.CHARGING && state != States.DEAD)
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
        if (_sprite.Animation == "Recover")
        {
            state = States.AIR;
        }
    }

    public void _on_ChargingTimer_timeout()
    {
        if (state == States.CHARGING)
        {
            SetShooting();
        }
    }

}
