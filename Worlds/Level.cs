using Godot;
using System;

public class Level : Node2D
{
    private TileMap _destructableTilemap;

    public override void _Ready()
    {
        _destructableTilemap = GetNode<TileMap>("DestructableTilemap");
    }

    public void destroyTile(Vector2 destroyPos)
    {
        Vector2 gridPos = _destructableTilemap.WorldToMap(destroyPos);
        _destructableTilemap.SetCellv(gridPos, -1);
    }
}
