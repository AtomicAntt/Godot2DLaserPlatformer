using Godot;
using System;

public class Player : KinematicBody2D
{
    public enum States {AIR, FLOOR, DEAD};

    [Export]
    public States state = States.AIR;

    [Export]
    public Vector2 velocity = Vector2.Zero;

    private const int _Speed = 330;
    private const int _Gravity = 1300;
    private const int _Jump = -600;
    
    public override void _Ready()
    {
        
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

    public override void _PhysicsProcess(float delta)
    {
        velocity = MoveAndSlide(velocity, Vector2.Up);
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
    }
}
