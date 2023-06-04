using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MapVisualizer : MonoBehaviour
{
    [SerializeField]
    public Tilemap _floorTileMap, _wallTileMap;

    [SerializeField]
    private TileBase _floorTile, _wallTop, wallSideRight, wallSideLeft, wallBottom, wallFull, 
        wallInnerCornerDownLeft, wallInnerCornerDownRight, 
        wallDiagonalCornerDownRight, wallDiagonalCornerDownLeft, wallDiagonalCornerUpRight, wallDiagonalCornerUpLeft;

    public void Draw(IEnumerable<Vector2Int> floorPos)
    {
        DrawTiles(floorPos, _floorTileMap, _floorTile);
    }

    private void DrawTiles(IEnumerable<Vector2Int> positions, Tilemap tileMap, TileBase tile)
    {
        foreach (var pos in positions)
        {
            DrawSingleTile(tileMap, tile, pos);
        }
    }

    private void DrawSingleTile(Tilemap tileMap, TileBase tile, Vector2Int position)
    {
        var tilePos = tileMap.WorldToCell((Vector3Int) position);
        tileMap.SetTile(tilePos, tile);
    }

    public void Clear()
    {
        _floorTileMap.ClearAllTiles();
        _wallTileMap.ClearAllTiles();
    }

    internal void DrawSingleBasicWall(Vector2Int position, string binType)
    {
        int typeAsInt = Convert.ToInt32(binType, 2);
        TileBase tileBase = WallTypesHelper.wallTop.Contains(typeAsInt) ? _wallTop :
                   WallTypesHelper.wallSideRight.Contains(typeAsInt) ? wallSideRight :
                   WallTypesHelper.wallSideLeft.Contains(typeAsInt) ? wallSideLeft :
                   WallTypesHelper.wallBottm.Contains(typeAsInt) ? wallBottom :
                   WallTypesHelper.wallFull.Contains(typeAsInt) ? wallFull :
                   null;

        if (tileBase != null)
            DrawSingleTile(_wallTileMap, tileBase, position);
    }

    internal void DrawSingleCornerWall(Vector2Int pos, string binType)
    {
        int typeAsInt = Convert.ToInt32(binType, 2);
        TileBase tileBase = null;

        if (WallTypesHelper.wallInnerCornerDownLeft.Contains(typeAsInt))
            tileBase = wallInnerCornerDownLeft;
        else if (WallTypesHelper.wallInnerCornerDownRight.Contains(typeAsInt))
            tileBase = wallInnerCornerDownRight;
        else if (WallTypesHelper.wallDiagonalCornerDownLeft.Contains(typeAsInt))
            tileBase = wallDiagonalCornerDownLeft;
        else if (WallTypesHelper.wallDiagonalCornerDownRight.Contains(typeAsInt))
            tileBase = wallDiagonalCornerDownRight;
        else if (WallTypesHelper.wallDiagonalCornerUpRight.Contains(typeAsInt))
            tileBase = wallDiagonalCornerUpRight;
        else if (WallTypesHelper.wallDiagonalCornerUpLeft.Contains(typeAsInt))
            tileBase = wallDiagonalCornerUpLeft;
        else if (WallTypesHelper.wallFullEightDirections.Contains(typeAsInt))
            tileBase = wallFull;
        else if (WallTypesHelper.wallBottmEightDirections.Contains(typeAsInt))
            tileBase = wallBottom;

        if (tileBase != null)
            DrawSingleTile(_wallTileMap, tileBase, pos);
    }
}
