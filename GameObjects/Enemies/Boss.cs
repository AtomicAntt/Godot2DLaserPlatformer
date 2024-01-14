using Godot;
using System;

public class Boss : StaticBody2D
{
    private Main _main;
    
    private AnimatedSprite _sprite;

    public override void _Ready()
    {
        _main = GetTree().GetNodesInGroup("main")[0] as Main;

        _sprite = GetNode<AnimatedSprite>("AnimatedSprite");

        if (_main.helperBotDestroyed)
        {
            _sprite.Play("Boss");
        }
        else
        {
            _sprite.Play("Normal");
        }
    }


}
