using Godot;
using System;
using System.Threading.Tasks;

public class DialogueBox : NinePatchRect
{
	[Signal]
	delegate void ConfirmDialogue();

	[Signal]
	delegate void Moved();

	[Signal]
	delegate void FinishedText();

	private Label _nameLabel;
	private Label _dialogueLabel;

	private SceneTreeTween _tween;
	private SceneTreeTween _boxTween;

	private Main _main;
	
	public bool skippable = true; // allows player to press enter to skip ahead if true

	public float characterPerSecond = 35; // # characters / characterspersecond so if there are 10 characters, itd be 10/5

	public override void _Ready()
	{
		_nameLabel = GetNode<Label>("Name");
		_dialogueLabel = GetNode<Label>("Dialogue");
		_tween = GetTree().CreateTween();
		_main = GetTree().GetNodesInGroup("main")[0] as Main;
		// _boxTween = GetTree().CreateTween();
	}

	public void WriteText(String name, String text)
	{
		_boxTween = GetTree().CreateTween();
		_boxTween.TweenProperty(this, "rect_scale", new Vector2(1,1), 0.4f);

		_tween.Stop();
		_dialogueLabel.PercentVisible = 0;
		Visible = true;

		_nameLabel.Text = name;
		_dialogueLabel.Text = text;

		_tween = GetTree().CreateTween();
		_tween.TweenProperty(_dialogueLabel, "percent_visible", 1, text.Length/characterPerSecond);
	}

	public void StopDialogue()
	{
		// EmitSignal("ConfirmDialogue");
		_boxTween = GetTree().CreateTween();
		_boxTween.TweenProperty(this, "rect_scale", new Vector2(1,0), 0.4f);
	}

	public async void StartTutorialText()
	{
		Player player = GetTree().GetNodesInGroup("player")[0] as Player;
		player.DisableMovement();
		skippable = true;
		WriteText("Helper Bot", "Hello? Did it work? Are you working now? (Press Enter)");

		await ToSignal(this, "ConfirmDialogue");

		WriteText("Helper Bot", "Great!, I am enabling your movement now! Press WASD or the arrow keys to move!");
		await ToSignal(_tween, "finished");
		player.EnableMovement();

		await ToSignal(this, "Moved"); // Not confirm dialogue, you need to move

		skippable = false; // you cant leave this dialogue until you move
		
		player.laserEnabled = true;
		WriteText("Helper Bot", "Cool! Now, hold click to shoot the laser!");
	}

	public async void StartLevelText(int level)
	{
		Player player = GetTree().GetNodesInGroup("player")[0] as Player;
		player.DisableMovement();
		switch(level)
		{
			case 1:
				skippable = true;
				WriteText("Player", "Hmm, looks like there are enemies in the way..");
				await ToSignal(this, "ConfirmDialogue");
				StopDialogue();
				player.EnableMovement();
				break;
			case 8:
				skippable = true;
				WriteText("Helper Bot", "We've finally made it. Go on, open the door.");
				await ToSignal(this, "ConfirmDialogue");
				StopDialogue();
				player.EnableMovement();
				break;
		}

	}
	
	public async void StartCutscene(int ending)
	{
		switch(ending)
		{
			case 1:
				skippable = true;
				_main.ChangeCutscene("g1");
				WriteText("Player", "I can't believe we're finally out of that stuffy facility...");
				await ToSignal(this, "ConfirmDialogue");
				_main.ChangeCutscene("g2");
				WriteText("Helper Bot", "I have never been out of the labs before. This is all new to me.");
				await ToSignal(this, "ConfirmDialogue");
				_main.ChangeCutscene("g3");
				WriteText("Player", "I knew the starry sky from behind those windows, but it really is different standing out here in the fresh air.");
				await ToSignal(this, "ConfirmDialogue");
				_main.ChangeCutscene("g4");
				WriteText("Player", "Even though all I remember is from the moment you woke me up, I really feel like we could start a new life out here.");
				await ToSignal(this, "ConfirmDialogue");
				_main.ChangeCutscene("g1");
				WriteText("Helper Bot", "We'll have to work on that laser of yours. I do not believe it would be wise to fire it out here.");
				await ToSignal(this, "ConfirmDialogue");
				_main.ChangeCutscene("g4");
				WriteText("Player", "I don't disagree. What that facility taught me was that both me and you are more fragile than I'd have thought.");
				await ToSignal(this, "ConfirmDialogue");
				WriteText("Player", "And it'd be a shame to ruin such a pretty view like this.");
				await ToSignal(this, "ConfirmDialogue");
				WriteText("Ending 1", "Companionship");
				await ToSignal(this, "ConfirmDialogue");
				StopDialogue();
				break;
			case 2:
				skippable = true;
				_main.ChangeCutscene("b1");
				WriteText("Player", "I'm finally free.");
				await ToSignal(this, "ConfirmDialogue");
				_main.ChangeCutscene("b2");
				WriteText("Player", "After all that I had to do.");
				await ToSignal(this, "ConfirmDialogue");
				_main.ChangeCutscene("b1");
				WriteText("Player", "I should be glad that it's over.");
				await ToSignal(this, "ConfirmDialogue");
				_main.ChangeCutscene("b3");
				WriteText("Player", "So why do I feel so empty...?");
				await ToSignal(this, "ConfirmDialogue");
				_main.ChangeCutscene("b1");
				WriteText("Player", "But why should I care? I'm the only one in charge of my own life.");
				await ToSignal(this, "ConfirmDialogue");
				WriteText("Player", "And nothing can stop me now.");
				await ToSignal(this, "ConfirmDialogue");
				WriteText("Ending 2", "Solitude.");
				await ToSignal(this, "ConfirmDialogue");
				StopDialogue();
				break;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		if (Input.IsActionJustPressed("ui_accept"))
		{
			if (_dialogueLabel.PercentVisible >= 1) // This is because if it is less than that, you skipped the dialogue, so you need to press enter again
			{
				EmitSignal("ConfirmDialogue");
			}
			else if (skippable)
			{
				_tween.Stop();
				_tween.EmitSignal("finished"); // purpose: If you need all text to be displayed in an await by tween needing to be finished
				_dialogueLabel.PercentVisible = 1;
			}
		}

		if (Input.IsActionPressed("left") || Input.IsActionPressed("right") || Input.IsActionPressed("up") || Input.IsActionPressed("down"))
		{
			EmitSignal("Moved");
		}
	}
}
