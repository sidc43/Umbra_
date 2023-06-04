using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class WallGenerator
{
    public static void CreateWalls(HashSet<Vector2Int> floorPositions, MapVisualizer mapVisualizer)
    {
        var basicWallPos = FindWallsInDir(floorPositions, Direction2D.cardinalDirList);
        var cornerWallPos = FindWallsInDir(floorPositions, Direction2D.diagonalDirList);

        CreateBasicWall(mapVisualizer, basicWallPos, floorPositions);
        CreateCornerWalls(mapVisualizer, cornerWallPos, floorPositions);
    }

    private static void CreateCornerWalls(MapVisualizer mapVisualizer, HashSet<Vector2Int> cornerWallPos, HashSet<Vector2Int> floorPositions)
    {
        foreach (var pos in cornerWallPos)
        {
            string neighborsBinType = "";
            foreach (var dir in Direction2D.eightDirList)
            {
                var neighborPos = pos + dir;
                if (floorPositions.Contains(neighborPos))
                    neighborsBinType += "1";
                else
                    neighborsBinType += "0";
            }
            mapVisualizer.DrawSingleCornerWall(pos, neighborsBinType);
        }
    }

    private static void CreateBasicWall(MapVisualizer mapVisualizer, HashSet<Vector2Int> basicWallPos, HashSet<Vector2Int> floorPositions)
    {
        foreach (var position in basicWallPos)
        {
            string neighborsBinType = "";
            foreach (var dir in Direction2D.cardinalDirList)
            {
                var neighborPos = position + dir;
                if (floorPositions.Contains(neighborPos))
                    neighborsBinType += "1";
                else
                    neighborsBinType += "0";
            }
            mapVisualizer.DrawSingleBasicWall(position, neighborsBinType);
        }
    }

    private static HashSet<Vector2Int> FindWallsInDir(HashSet<Vector2Int> floorPositions, List<Vector2Int> directionList)
    {
        HashSet<Vector2Int> wallPositions = new HashSet<Vector2Int>();

        foreach (var position in floorPositions)
        {
            foreach (var direction in directionList)
            {
                var neightborPosition = position + direction;
                if (!floorPositions.Contains(neightborPosition))
                    wallPositions.Add(neightborPosition);
            }
        }

        return wallPositions;
    }
}
