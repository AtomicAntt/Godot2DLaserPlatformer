using Godot;
using System;

public class Main : Node
{
    // Onready variables that can be referenced
    private Control _mainMenu; 
    private Node2D _levels;

    private Label _keycardStatus;

    [Export]
    public int currentLevel = 0;

    [Export]
    public int numLevels = 5;

    public float restartConfirmation = 0;


    private Node2D _levelInstance;

    public override void _Ready()
    {
        _mainMenu = GetNode<Control>("MainMenu");
        _levels = GetNode<Node2D>("Levels");
        _keycardStatus = GetNode<Label>("Levels/CanvasLayer/Control/KeycardStatus");

        LoadLevel("Level0");
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

    // door calls this
    public bool LoadNextLevel()
    {
        if ((currentLevel >= numLevels))
        {
            GD.Print("current level is already at or somehow greater than numlevels, so i cant load the next level!");
            return false;
        }

        LoadLevel("Level" + (currentLevel+=1));
        return true;
    }

    public void CheckKeycardStatus()
    {
        Player player = GetTree().GetNodesInGroup("player")[0] as Player;
        if (!IsInstanceValid(player))
        {
            return;
        }
        if (GetTree().GetNodesInGroup("keycard").Count == 0 && !player.hasKeycard)
        {
            _keycardStatus.Visible = true;
            _keycardStatus.Text = "There are no keycards left in the map! Hold 'R' to restart the level!";
        }
        else if (player.hasKeycard){
            _keycardStatus.Visible = true;
            _keycardStatus.Text = "Keycard obtained! Find the door to proceed to the next level!";
        }
        else
        {
            _keycardStatus.Visible = false;
        }
    }

    public void RestartLevel()
    {
        LoadLevel("Level" + (currentLevel));
    }

    public override void _PhysicsProcess(float delta)
    {
        CheckKeycardStatus();
        if (Input.IsActionPressed("restart"))
        {
            restartConfirmation += delta;
            if (restartConfirmation >= 0.2)
            {
                RestartLevel();
                restartConfirmation = 0;
            }
        }
        else
        {
            restartConfirmation = 0;
        }
    }
}
