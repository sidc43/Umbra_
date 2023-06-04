using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class SimpleRandWalkMapGen : AbstractDungeonGenerator
{
    [SerializeField]
    protected SimpleRandomWalkSO randomWalkParameters;

    protected override void RunProceduralGeneration()
    {
        HashSet<Vector2Int> floorPos = StartRandomWalk(randomWalkParameters, startPos);
        mapVisualizer.Clear();
        mapVisualizer.Draw(floorPos);
        WallGenerator.CreateWalls(floorPos, mapVisualizer);
    }

    protected HashSet<Vector2Int> StartRandomWalk(SimpleRandomWalkSO parameters, Vector2Int position)
    {
        var currPos = position;
        HashSet<Vector2Int> floorPos = new HashSet<Vector2Int>();

        for (int i = 0; i < randomWalkParameters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(currPos, randomWalkParameters.walkLength);
            floorPos.UnionWith(path);

            if (randomWalkParameters.startRandomlyEachIteration)
                currPos = floorPos.ElementAt(Random.Range(0, floorPos.Count));
        }

        return floorPos;
    }
}
