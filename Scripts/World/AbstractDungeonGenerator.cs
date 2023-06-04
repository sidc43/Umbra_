using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected MapVisualizer mapVisualizer = null;
    [SerializeField]
    protected Vector2Int startPos = Vector2Int.zero;

    public void GenerateDungeon()
    {
        mapVisualizer.Clear();
        RunProceduralGeneration();
    }

    protected abstract void RunProceduralGeneration();
}
