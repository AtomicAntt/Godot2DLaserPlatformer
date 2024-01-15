using Godot;
using System;

public class Boss : StaticBody2D
{
	private Main _main;
	
	private Timer _timer;
	
	private float _time = 1.50f;
	
	private AnimatedSprite _sprite;
	
	private Label _status;
	
	private int summonCount;
	
//	private Player player;
	

	public override void _Ready()
	{
		_main = GetTree().GetNodesInGroup("main")[0] as Main;
//		player = GetTree().GetNodesInGroup("player")[0] as Player;

		_sprite = GetNode<AnimatedSprite>("AnimatedSprite");
		_timer = GetNode<Timer>("Timer");
		_status = GetNode<Label>("Label");
		_sprite.Play("Startup");
		summonCount = _main.totalCount / 3;
//	if (_main.helperBotDestroyed)
//		{
//			_sprite.Play("Boss");
//		}
//		else
//		{
//			_sprite.Play("Normal");
	}


	public void startFight()
	{
		// refresh seed
		GD.Randomize();
		_timer.Start(2.0f);
	}
//		

	
	private void _summon(int type){
	_status.Text = summonCount + " enemies remaining!";
// insert match statement to decide which enemy to spawn
// pick a random spawn point using math
// spawn in path2d, give it a random pre-made path2d resource, instance pathfollow2d as child,
// and instance actual enemy as child of pathfollow2d if applicable
// otherwise just spawn it in
	summonCount -= 1;
	var random = new RandomNumberGenerator();
	random.Randomize();
	Vector2 spawn = new Vector2((float) random.RandiRange(-150, 150), (float) random.RandiRange(-25, -45));
//	while (Math.Abs(spawn.x - player.GlobalPosition.x) > 20 | Math.Abs(spawn.y - player.GlobalPosition.y) > 10){
//		spawn = new Vector2((float) random.RandiRange(-300, 300), (float) random.RandiRange(25, 45));
//	}
	String enemyPath;
	PackedScene enemyResource;
	KinematicBody2D enemyInstance;
	Path2D pathing;
	PathFollow2D follow;
	String pathType = "res://GameObjects/Enemies/Curve2D" + (random.RandiRange(0, 3)) + ".tres";
	switch(type)
		{
			// load small drone
			case 0:
				pathing = new Path2D();
				follow = new PathFollow2D();
				this.AddChild(pathing);
				// reroll because vertical line does not work well with small enemies
				while (pathType == "res://GameObjects/Enemies/Curve2D0.tres")
					{
					pathType = "res://GameObjects/Enemies/Curve2D" + (random.RandiRange(0, 3)) + ".tres";
					}
				pathing.Curve = GD.Load<Curve2D>(pathType);
				pathing.AddChild(follow);
				follow.Position = spawn;
				follow.Rotate = false;
				enemyPath = "res://GameObjects/Enemies/EnemySmallDrone.tscn";
				enemyResource = GD.Load<PackedScene>(enemyPath);
				if (enemyResource != null)
				{
					enemyInstance = enemyResource.Instance<KinematicBody2D>();
					follow.AddChild(enemyInstance);
					enemyInstance.RotationDegrees = -180;
				}
				break;
			// load big drone
			case 1:
				pathing = new Path2D();
				follow = new PathFollow2D();
				this.AddChild(pathing);
				pathing.Curve = GD.Load<Curve2D>(pathType);
				pathing.AddChild(follow);
				follow.Position = spawn;
				follow.Rotate = false;
				enemyPath = "res://GameObjects/Enemies/Enemy.tscn";
				enemyResource = GD.Load<PackedScene>(enemyPath);
				if (enemyResource != null)
				{
					enemyInstance = enemyResource.Instance<KinematicBody2D>();
					follow.AddChild(enemyInstance);
					enemyInstance.RotationDegrees = -180;
				}
				break;
			// load torus
			case 2:
				pathing = new Path2D();
				follow = new PathFollow2D();
				this.AddChild(pathing);
				// reroll because vertical line does not work well with small enemies
				while (pathType == "res://GameObjects/Enemies/Curve2D0.tres")
					{
					pathType = "res://GameObjects/Enemies/Curve2D" + (random.RandiRange(0, 3)) + ".tres";
					}
				pathing.Curve = GD.Load<Curve2D>(pathType);
				pathing.AddChild(follow);
				follow.Position = spawn;
				follow.Rotate = false;
				enemyPath = "res://GameObjects/Enemies/EnemyTorus.tscn";
				enemyResource = GD.Load<PackedScene>(enemyPath);
				if (enemyResource != null)
				{
					enemyInstance = enemyResource.Instance<KinematicBody2D>();
					follow.AddChild(enemyInstance);
					enemyInstance.RotationDegrees = -180;
				}
				break;
			// load small turret
			case 3:
				enemyPath = "res://GameObjects/Enemies/EnemySmallTurret.tscn";
				enemyResource = GD.Load<PackedScene>(enemyPath);
				if (enemyResource != null)
				{
					enemyInstance = enemyResource.Instance<KinematicBody2D>();
					this.AddChild(enemyInstance);
					enemyInstance.Position = spawn;
				}
					break;
			// load big turret
			case 4:
				enemyPath = "res://GameObjects/Enemies/EnemyTurret.tscn";
					enemyResource = GD.Load<PackedScene>(enemyPath);
				if (enemyResource != null)
				{
					enemyInstance = enemyResource.Instance<KinematicBody2D>();
					this.AddChild(enemyInstance);
					enemyInstance.Position = spawn;
				}
				break;
			// load humanoid
			case 5:
				enemyPath = "res://GameObjects/Enemies/EnemyHumanoid.tscn";
				enemyResource = GD.Load<PackedScene>(enemyPath);
				if (enemyResource != null)
				{
					enemyInstance = enemyResource.Instance<KinematicBody2D>();
					this.AddChild(enemyInstance);
					((EnemyHumanoid) enemyInstance).setLaser(70, 10);
					((EnemyHumanoid) enemyInstance).speed = 100;
					enemyInstance.Position = spawn;
				}
				break;
			}
//		String enemyPath = "res://GameObjects/Enemy.tscn";
//			PackedScene enemyResource = GD.Load<PackedScene>(enemyPath);
//			if (enemyResource != null && (IsInstanceValid(_spawn)))
//			{
//				_enemyInstance = enemyResource.Instance<KinematicBody2D>();
//				this.AddChild(_enemyInstance);
//				_enemyInstance.Position = _spawn.Position;
//			}
	}
	
	// boss dies and player can now open door
	private void _dead()
	{
		Player player = GetTree().GetNodesInGroup("player")[0] as Player;
		player.hasKeycard = true;
		_status.Text = "Shutting down...\n";
		_sprite.Play("Death");
	}
	
	private void _on_Timer_timeout()
	{
		// checks if still summoning or not
		if (summonCount > 0 && GetTree().GetNodesInGroup("enemy").Count <= 10)
		{
			_status.Text = "Now summoning...\n" + summonCount + " enemies remaining!";
			_sprite.Play("BossSummon");
			_timer.Start(_time);
		}
		else if (summonCount <=0)
		{
			_dead();
			_timer.Stop();
		}
		else
		{
			_status.Text = "Destroy more enemies to progress...\n" + summonCount + " enemies remaining!";
			_sprite.Play("BossBreak");
			_timer.Start(_time);
		}
	}
	
	private void _on_AnimatedSprite_animation_finished()
	{
		if (_sprite.Animation == "BossSummon")
		{
			// picks random number, converts it to signed int, restricts to 0-5
			_summon((int) GD.Randi() % 6);
			_sprite.Play("Boss");
		}
		else if (_sprite.Animation == "BossBreak")
		{
			// boss idles after break animation
			_sprite.Play("Boss");
		}
		else if (_sprite.Animation == "Death"){
			_status.Text = "You can now leave through the door.";
		}
	}
}
