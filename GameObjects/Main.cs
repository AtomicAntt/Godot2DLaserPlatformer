using Godot;
using System;

public class Main : Node
{
    // Onready variables that can be referenced
    private Control _mainMenu; 
    private Node2D _levels;


    private Node2D _levelInstance;

    public override void _Ready()
    {
        _mainMenu = GetNode<Control>("MainMenu");
        _levels = GetNode<Node2D>("Levels");

        LoadLevel("Level1");
    }

    public void UnloadLevel() 
    {
        if (IsInstanceValid(_levelInstance)){
            _levelInstance.QueueFree();
        }
    }

    public void LoadLevel(String levelName)
    {
        UnloadLevel();

        String levelPath = "res://Worlds/" + levelName + ".tscn";
        PackedScene levelResource = GD.Load<PackedScene>(levelPath);
        if (levelResource != null){
            _levelInstance = levelResource.Instance<Node2D>();
            _levels.AddChild(_levelInstance);
        }
    }
}
