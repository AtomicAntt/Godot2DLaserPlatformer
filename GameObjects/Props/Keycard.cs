using Godot;
using System;

public class Keycard : Area2D
{
    private Player _player;

    public override void _Ready()
    {
        _player = GetTree().GetNodesInGroup("player")[0] as Player;
    }

    public void Destroy()
    {
        // Main main = GetTree().GetNodesInGroup("main")[0] as Main;
        // main.CheckKeycardStatus();
        QueueFree();
    }

    public void _on_Keycard_body_entered(Node2D body)
    {
        if (body.IsInGroup("player"))
        {
            _player.hasKeycard = true;
            // Main main = GetTree().GetNodesInGroup("main")[0] as Main;
            // main.CheckKeycardStatus();
            QueueFree();
        }
    }
}
