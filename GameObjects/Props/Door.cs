using Godot;
using System;

public class Door : Area2D
{
	private Player _player;
	private Main _main;

	private Label _label;
	public bool canEnter = false;

	public override void _Ready()
	{
		_player = GetTree().GetNodesInGroup("player")[0] as Player;
		_main = GetTree().GetNodesInGroup("main")[0] as Main;

		_label = GetNode<Label>("Label");


	}
	public void _on_Door_body_entered(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			_player = body as Player;
			_label.Visible = true;
			if (_player.hasKeycard)
			{
				canEnter = true;
				_label.Text = "Press 'E' to proceed to the next level!";
			}
			else
			{
				canEnter = false;
				_label.Text = "You need a keycard to open this door!";
			}
		}
	}

	public void _on_Door_body_exited(Node2D body)
	{
		if (body.IsInGroup("player"))
		{
			canEnter = false;
			_label.Visible = false;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event.IsActionPressed("interact") && _player.hasKeycard && canEnter)
		{
			_player.hasKeycard = false;
			_main.LoadNextLevel();
		}
	}


}
