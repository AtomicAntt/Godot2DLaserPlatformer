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

    public bool skippable = true; // allows player to press enter to skip ahead if true

    public float characterPerSecond = 35; // # characters / characterspersecond so if there are 10 characters, itd be 10/5

    public override void _Ready()
    {
        _nameLabel = GetNode<Label>("Name");
        _dialogueLabel = GetNode<Label>("Dialogue");
        _tween = GetTree().CreateTween();
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
        _boxTween.TweenProperty(this, "rect_scale", new Vector2(0,0), 0.4f);
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

        WriteText("Helper Bot", "Cool! Now, hold click to shoot the laser!");
    }

    public async void StartLevelText(int level)
    {
        switch(level)
        {
            case 1:
                Player player = GetTree().GetNodesInGroup("player")[0] as Player;
                player.DisableMovement();
                skippable = true;
                WriteText("Player", "Hmm, looks like there are enemies in the way..");
                await ToSignal(this, "ConfirmDialogue");
                StopDialogue();
                player.EnableMovement();
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
