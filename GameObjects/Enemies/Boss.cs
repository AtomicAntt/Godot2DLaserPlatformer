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
		_sprite.Play("Startup");

//		if (_main.helperBotDestroyed)
//		{
//			_sprite.Play("Boss");
//		}
//		else
//		{
//			_sprite.Play("Normal");
//		}
	}
	
//	private void summon(int type){
// insert match statement to decide which enemy to spawn
// pick a random spawn point either using position2d or math
// spawn in path2d, give it a random pre-made path2d resource, instance pathfollow2d as child,
// and instance actual enemy as child of pathfollow2d if applicable
// otherwise just spawn it in
//		String enemyPath = "res://GameObjects/Enemy.tscn";
//			PackedScene enemyResource = GD.Load<PackedScene>(enemyPath);
//			if (enemyResource != null && (IsInstanceValid(_spawn)))
//			{
//				_enemyInstance = enemyResource.Instance<KinematicBody2D>();
//				this.AddChild(_enemyInstance);
//				_enemyInstance.Position = _spawn.Position;
//	}
	
	// boss dies and player can now open door
	private void dead()
	{
		Player player = GetTree().GetNodesInGroup("player")[0] as Player;
		player.hasKeycard = true;
		_sprite.Play("Death");
	}


}
