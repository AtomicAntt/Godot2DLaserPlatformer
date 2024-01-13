using Godot;
using System;

public class Level : Node2D
{
    private TileMap _destructableTilemap;
    private TileMap _indestructableTilemap;
    private TileMap _backgroundTilemap;

    public override void _Ready()
    {
        _destructableTilemap = GetNode<TileMap>("DestructableTilemap");
        _backgroundTilemap = GetNode<TileMap>("BackgroundTilemap");
        _indestructableTilemap = GetNode<TileMap>("IndestructableTilemap");


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
