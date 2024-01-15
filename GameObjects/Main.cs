using Godot;
using System;

public class Main : Node
{
	// Onready variables that can be referenced
	private Control _mainMenu; 
	private Node2D _levels;
	private DialogueBox _dialogueBox;

	private CanvasLayer _canvasLayer;
	private AnimationPlayer _animationPlayer;
	
	private VBoxContainer _ui;

	private Label _keycardStatus;
	
	private Sprite _keycardIcon;
	
	private AnimatedSprite _cutscene;

	public bool inGame = false;

	[Export]
	public int currentLevel = 0;

	[Export]
	public int numLevels = 7;

	public float restartConfirmation = 0;

	public bool helperBotDestroyed = false; // Purpose: If you load next level while the instance of helper bot is dead, this will activate a boss at the end

	private Node2D _levelInstance;

	private AudioStreamPlayer _gameMusic;
	private AudioStreamPlayer _mainMenuMusic;
	private AudioStreamPlayer _hover;
	private AudioStreamPlayer _confirm;

	public override void _Ready()
	{
		_mainMenu = GetNode<Control>("MainMenu");
		_levels = GetNode<Node2D>("Levels");
		_ui = GetNode<VBoxContainer>("Levels/CanvasLayer/Control/VBoxContainer");
		_keycardStatus = GetNode<Label>("Levels/CanvasLayer/Control/KeycardStatus");
		_keycardIcon = GetNode<Sprite>("Levels/CanvasLayer/Control/KeycardIcon");
		_dialogueBox = GetNode<DialogueBox>("Levels/CanvasLayer/Control/DialogueBox");
		_cutscene = GetNode<AnimatedSprite>("Levels/CanvasLayer/Control/AnimatedSprite");
		_canvasLayer = GetNode<CanvasLayer>("Levels/CanvasLayer");
		_animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");

		_gameMusic = GetNode<AudioStreamPlayer>("GameMusic");
		_mainMenuMusic = GetNode<AudioStreamPlayer>("MainMenuMusic");
		_hover = GetNode<AudioStreamPlayer>("Hover");
		_confirm = GetNode<AudioStreamPlayer>("Confirm");

		_mainMenu.Visible = true;
		_cutscene.Visible = false;
		_canvasLayer.Visible = false;
		// LoadLevel("Level0");
	}

	public void _on_PlayButton_pressed()
	{
		_confirm.Play();
		_canvasLayer.Visible = true;
		_mainMenu.Visible = false;
		LoadLevel("Level0");
		inGame = true;
		_mainMenuMusic.Stop();
		_gameMusic.Play();
	}

	public void _on_Button_mouse_entered()
	{
		_hover.Play();
	}

	public void UnloadLevel() 
	{
		if (IsInstanceValid(_levelInstance)){
			_levelInstance.QueueFree();
			_keycardIcon.Visible = false;
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

			// This is to initialize dialogue during start of each game
			if (currentLevel == 0)
			{
				_dialogueBox.StartTutorialText();
			}
			else
			{
				_dialogueBox.StartLevelText(currentLevel);
			}

		}

	}

	// door calls this
	public async void LoadNextLevel()
	{
		// Purpose: When you go through the door, this function is called right, so we need to check if the helper bot is gonna be there
		if (GetTree().GetNodesInGroup("helperBot").Count > 0)
		{
			HelperBot helperBot = GetTree().GetNodesInGroup("helperBot")[0] as HelperBot;

			if (IsInstanceValid(helperBot))
			{
				if (helperBot.destroyed)
				{
					helperBotDestroyed = true;
				}
			}
		}

		if (currentLevel >= numLevels)
		{
			// load endings
			if ((currentLevel == 7))
			{
				// bad ending
				if (helperBotDestroyed)
				{
					currentLevel = 9;
					_keycardIcon.Visible = false;
					LoadLevel("Level8b");
					return;
				}
				else
				{
				// good ending
					currentLevel = 8;
					
					LoadLevel("Level8a");
					return;
				}
			}
			// call cutscene
			if ((currentLevel == 8))
			{
				_keycardStatus.Visible = false;
				_keycardIcon.Visible = false;
				_ui.Visible = false;
				_cutscene.Visible = true;
				_dialogueBox.StartCutscene(1);
				return;
			}
			if ((currentLevel == 9))
			{
				_keycardStatus.Visible = false;
				_keycardIcon.Visible = false;
				_ui.Visible = false;
				_cutscene.Visible = true;
				_dialogueBox.StartCutscene(2);
				return;
			}
			// invalid level
			else
			{
				GD.Print("current level is already at or somehow greater than numlevels, so i cant load the next level!");
				return;
			}
		}

		// CODE MOVED UP
		// // Purpose: When you go through the door, this function is called right, so we need to check if the helper bot is gonna be there
		// if (GetTree().GetNodesInGroup("helperBot").Count > 0)
		// {
		// 	HelperBot helperBot = GetTree().GetNodesInGroup("helperBot")[0] as HelperBot;

		// 	if (IsInstanceValid(helperBot))
		// 	{
		// 		if (helperBot.destroyed)
		// 		{
		// 			helperBotDestroyed = true;
		// 		}
		// 	}
		// }

		_animationPlayer.Play("fadeIn");
		await ToSignal(_animationPlayer, "animation_finished");
		LoadLevel("Level" + (currentLevel+=1));
		_animationPlayer.PlayBackwards("fadeIn");
		return;
	}

	public void ChangeCutscene(String name){
		_cutscene.Play(name);
	}
	
	public void CheckKeycardStatus()
	{
		Player player = GetTree().GetNodesInGroup("player")[0] as Player;
		if (!IsInstanceValid(player))
		{
			return;
		}
		if (currentLevel >= 8){
			return;
		}
		else if (player.hasKeycard)
		{
			_keycardIcon.Visible = true;
			_keycardStatus.Visible = true;
			_keycardStatus.Text = "Keycard obtained! Find the door to proceed to the next level!";
		}
		else if (GetTree().GetNodesInGroup("keycard").Count == 0 && !player.hasKeycard)
		{
			_keycardStatus.Visible = true;
			_keycardStatus.Text = "There are no keycards left in the map! Hold 'R' to restart the level!";
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
		if (inGame)
		{
			CheckKeycardStatus();
			if (Input.IsActionPressed("restart") && currentLevel != 0)
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

	public void DestroyTileSound()
	{
		AudioStreamPlayer destroyTile = GetNode<AudioStreamPlayer>("DestroyTile" + (GD.Randi() % 3 + 1));
		// AudioStreamPlayer destroyTile = GetNode<AudioStreamPlayer>("DestroyTile2");
		destroyTile.Play();
	}
}
