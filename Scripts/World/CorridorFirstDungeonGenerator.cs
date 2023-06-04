using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CorridorFirstDungeonGenerator : SimpleRandWalkMapGen
{
    [SerializeField]
    private int corridorLength = 10, corridorCount = 5;

    [SerializeField]
    [Range(0.1f, 1)]
    private float roomPercent = 0.7f;

    protected override void RunProceduralGeneration()
    {
        CorridorFirstDungeonGeneration();
    }

    private void CorridorFirstDungeonGeneration()
    {
        HashSet<Vector2Int> floorPos = new HashSet<Vector2Int>();
        HashSet<Vector2Int> potentialRoomPos = new HashSet<Vector2Int>();

        List<List<Vector2Int>> corridors = CreateCorridors(floorPos, potentialRoomPos);

        HashSet<Vector2Int> roomPos = CreateRooms(potentialRoomPos);

        List<Vector2Int> deadEnds = FindAllDeadEnds(floorPos);

        CreateRoomsAtDeadEnds(deadEnds, roomPos);

        floorPos.UnionWith(roomPos);

        for (int i = 0; i < corridors.Count; i++)
        {
            //corridors[i] = IncreaseCorridorSizeByOne(corridors[i]);
            corridors[i] = IncreaseCorridorBrushBy3(corridors[i]);
            floorPos.UnionWith(corridors[i]);
        }

        mapVisualizer.Draw(floorPos);
        WallGenerator.CreateWalls(floorPos, mapVisualizer);
    }

    private List<Vector2Int> IncreaseCorridorBrushBy3(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        for (int i = 1; i < corridor.Count; i++)
        {
            for (int x = -1; x < 2; x++)
            {
                for (int y = -1; y < 2; y++)
                {
                    newCorridor.Add(corridor[i-1] + new Vector2Int(x, y));
                }
            }
        }
        return newCorridor;
    }

    private List<Vector2Int> IncreaseCorridorSizeByOne(List<Vector2Int> corridor)
    {
        List<Vector2Int> newCorridor = new List<Vector2Int>();
        Vector2Int prevDir = Vector2Int.zero;

        for (int i = 1; i < corridor.Count; i++)
        {
            Vector2Int dirFromCell = corridor[i] - corridor[i - 1];
            if (prevDir != Vector2Int.zero && dirFromCell != prevDir)
            {
                // corner
                for (int x = -1; x < 2; x++)
                {
                    for (int y = -1; y < 2; y++)
                    {
                        newCorridor.Add(corridor[i - 1] + new Vector2Int(x, y));
                    }
                }
                prevDir = dirFromCell;
            }
            else
            {
                // singe cell + 90 deg rotation
                Vector2Int newCorridorTileOffset = GetDirection90From(dirFromCell);
                newCorridor.Add(corridor[i - 1]);
                newCorridor.Add(corridor[i - 1] + newCorridorTileOffset);
            }
        }
        return newCorridor;
    }

    private Vector2Int GetDirection90From(Vector2Int dir)
    {
        if (dir == Vector2Int.up)
            return Vector2Int.right;
        if (dir == Vector2Int.right)
            return Vector2Int.down;
        if (dir == Vector2Int.down)
            return Vector2Int.left;
        if (dir == Vector2Int.left)
            return Vector2Int.up;
        return Vector2Int.zero;
    }

    private void CreateRoomsAtDeadEnds(List<Vector2Int> deadEnds, HashSet<Vector2Int> roomPos)
    {
        foreach (var pos in deadEnds)
        {
            if (!roomPos.Contains(pos))
            {
                var room = StartRandomWalk(randomWalkParameters, pos);
                roomPos.UnionWith(room);
            }
        }
    }

    private List<Vector2Int> FindAllDeadEnds(HashSet<Vector2Int> floorPos)
    {
        List<Vector2Int> deadEnds = new List<Vector2Int>();
        foreach (var pos in floorPos)
        {
            int neighborCount = 0;
            foreach (var dir in Direction2D.cardinalDirList)
            {
                if (floorPos.Contains(pos + dir))
                    neighborCount++;
            }
            if (neighborCount == 1)
                deadEnds.Add(pos);
        }

        return deadEnds;
    }

    private HashSet<Vector2Int> CreateRooms(HashSet<Vector2Int> potentialRoomPos)
    {
        HashSet<Vector2Int> roomPositions = new HashSet<Vector2Int>();
        int roomCount = Mathf.RoundToInt(potentialRoomPos.Count * roomPercent);

        List<Vector2Int> roomsToCreate = potentialRoomPos.OrderBy(x => Guid.NewGuid()).Take(roomCount).ToList();

        foreach (var roomPosition in roomsToCreate)
        {
            var roomFloor = StartRandomWalk(randomWalkParameters, roomPosition);
            roomPositions.UnionWith(roomFloor);
        }

        return roomPositions;
    }

    private List<List<Vector2Int>> CreateCorridors(HashSet<Vector2Int> floorPos, HashSet<Vector2Int> potentialRoomPos)
    {
        var currPos = startPos;
        potentialRoomPos.Add(currPos);

        List<List<Vector2Int>> corridors = new List<List<Vector2Int>>();

        for (int i = 0; i < corridorCount; i++)
        {
            var corridor = ProceduralGenerationAlgorithms.RandomWalkCorridor(currPos, corridorLength);
            corridors.Add(corridor);
            currPos = corridor[corridor.Count - 1];
            potentialRoomPos.Add(currPos);
            floorPos.UnionWith(corridor);
        }
        return corridors;
    }
}
