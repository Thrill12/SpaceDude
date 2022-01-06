using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleRandomWalkDungeonGenerator : AbstractDungeonGenerator
{
    [SerializeField]
    protected SimpleRandomWalkSO randomWalkParameters;

    protected override void RunProceduralGeneration()
    {        
        HashSet<Vector2Int> floorPositions = RunRandomWalk(randomWalkParameters, startPos);
        tilemapVisualizer.Clear();
        tilemapVisualizer.PaintFloorTiles(floorPositions);
        WallGenerator.CreateWalls(floorPositions, tilemapVisualizer);
    }

    public override void Clear()
    {
        tilemapVisualizer.Clear();
    }

    protected HashSet<Vector2Int> RunRandomWalk(SimpleRandomWalkSO paremeters, Vector2Int position)
    {
        var currentPosition = position;
        HashSet<Vector2Int> floorPositions = new HashSet<Vector2Int>();
        for (int i = 0; i < paremeters.iterations; i++)
        {
            var path = ProceduralGenerationAlgorithms.SimpleRandomWalk(position, paremeters.walkLength);
            floorPositions.UnionWith(path);

            if (paremeters.startRandomlyEachIteration)
            {
                currentPosition = floorPositions.ElementAt(UnityEngine.Random.Range(0, floorPositions.Count));
            }
        }
        return floorPositions;
    }
}
