using Godot;
using System;

public class Level : Node2D
{
	private TileMap _destructableTilemap;
	private TileMap _indestructableTilemap;
	private TileMap _backgroundTilemap;
	
	private Main _main;
	private KinematicBody2D _botInstance;
	private Position2D _spawn;

	public override void _Ready()
	{
		_destructableTilemap = GetNode<TileMap>("DestructableTilemap");
		_backgroundTilemap = GetNode<TileMap>("BackgroundTilemap");
		_indestructableTilemap = GetNode<TileMap>("IndestructableTilemap");
		_spawn = GetNode<Position2D>("Position2D");
		_main = GetTree().GetNodesInGroup("main")[0] as Main;
		
		// checks if helper bot alive and spawns
		if (_main.helperBotDestroyed == false)
		{
			String botPath = "res://GameObjects/HelperBot.tscn";
			PackedScene botResource = GD.Load<PackedScene>(botPath);
			if (botResource != null && (IsInstanceValid(_spawn)))
			{
				_botInstance = botResource.Instance<KinematicBody2D>();
				this.AddChild(_botInstance);
				_botInstance.Position = _spawn.Position;
			}
		}
		


		// Purpose, put white tiles behind all tiles to account for corners showing the background
		foreach (Vector2 usedCell in _destructableTilemap.GetUsedCells())
		{
			_backgroundTilemap.SetCellv(usedCell, 1);
		}

	}

	public void destroyTile(Vector2 destroyPos)
	{
		Vector2 gridPos = _destructableTilemap.WorldToMap(destroyPos);
		if (_destructableTilemap.GetCellv(gridPos) != TileMap.InvalidCell)
		{
			_destructableTilemap.SetCellv(gridPos, -1);
			_backgroundTilemap.SetCellv(gridPos, 6);
		}

		if (_indestructableTilemap.GetCellv(gridPos) != TileMap.InvalidCell)
		{
			_indestructableTilemap.SetCellv(gridPos, 3);
		}
	}
}
