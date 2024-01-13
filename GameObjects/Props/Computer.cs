using Godot;
using System;

public class Computer : Area2D
{
    private Player _player;
    private Main _main;

    private Label _label;
    private AnimatedSprite _sprite;
    public bool canUse = false;

    public bool destroyed = false;
    public bool used = false;

    public override void _Ready()
    {
        _player = GetTree().GetNodesInGroup("player")[0] as Player;
        _main = GetTree().GetNodesInGroup("main")[0] as Main;

        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");
        _label = GetNode<Label>("Label");


    }
    public void _on_Computer_body_entered(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            _label.Visible = true;

            if (destroyed && !used)
            {
                canUse = false;
                _label.Text = "This destroyed computer can no longer be used to heal the player!";
            }
            else if (destroyed && used)
            {
                canUse = false;
                _label.Text = "This used computer has been destroyed.";
            }
            else if (used)
            {
                canUse = false;
                _label.Text = "You have already used this computer!";
            }
            else if (!used)
            {
                canUse = true;
                _label.Text = "Press 'e' to recover all damage taken!";
            }
        }
    }

    public void _on_Computer_body_exited(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            canUse = false;
            _label.Visible = false;
        }
    }

    public void Destroy()
    {
        _sprite.Play("Destroyed");
        canUse = false;
        destroyed = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("interact") && canUse)
        {
            _sprite.Play("Used");
            _player = GetTree().GetNodesInGroup("player")[0] as Player;
            canUse = false;
            used = true;
            _player.Recover();
            _label.Text = "You have already used this computer!";
        }
    }
}
