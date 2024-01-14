using Godot;
using System;

public class HelperBot : KinematicBody2D
{

    public enum States {FOLLOW, IDLE, DESTROYED}

    [Export]
    public States state = States.FOLLOW;
    public bool destroyed = false;
    public const int _Gravity = 1300;

    private CollisionShape2D _collisionShape2D;

    public Vector2 vectorOffsetRight = new Vector2(50, -56);
    public Vector2 vectorOffsetLeft = new Vector2(-50, -56);



    private Vector2 _velocity = Vector2.Zero;


    private Player _player;
    private AnimatedSprite _sprite;

    public override void _Ready()
    {
        _player = GetTree().GetNodesInGroup("player")[0] as Player;

        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _collisionShape2D = GetNode<CollisionShape2D>("CollisionShape2D");
    }

    public void Destroy()
    {
        state = States.DESTROYED;
        destroyed = true;
        SetCollisionMaskBit(0, true);

        DialogueBox dialogueBox = GetTree().GetNodesInGroup("dialogueBox")[0] as DialogueBox;
        dialogueBox.StopDialogue(); // Oh.. you just killed me, dead bots cant speak :(

    }

    public override void _PhysicsProcess(float delta)
    {
        switch (state)
        {
            case States.IDLE:
                _sprite.Play("Default");
                break;
            case States.FOLLOW:
                if (!IsInstanceValid(_player))
                {
                    _player = GetTree().GetNodesInGroup("player")[0] as Player;
                }
                _sprite.Play("Default");
                SceneTreeTween tween = GetTree().CreateTween();
                if (GetGlobalMousePosition().x > _player.GlobalPosition.x)
                {
                    tween.TweenProperty(this, "position", _player.GlobalPosition + vectorOffsetRight, 0.5f);
                    _sprite.FlipH = false;
                }
                else
                {
                    _sprite.FlipH = true;
                    tween.TweenProperty(this, "position", _player.GlobalPosition + vectorOffsetLeft, 0.5f);
                }

                break;
            case States.DESTROYED:
                _sprite.Play("Destroyed");
                _velocity.y += _Gravity;
                _velocity = MoveAndSlide(_velocity, Vector2.Up);
                break;
        }

        
    }

}
