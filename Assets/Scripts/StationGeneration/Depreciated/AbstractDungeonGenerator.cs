using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public abstract class AbstractDungeonGenerator : MonoBehaviour
{
    [SerializeField]
    protected TilemapVisualizer tilemapVisualizer = null;
    
    protected Vector2Int startPos;

    public void GenerateDungeon()
    {
        startPos = new Vector2Int((int)transform.position.x, (int)transform.position.y);
        RunProceduralGeneration();
        tilemapVisualizer.UpdateMesh();
    }

    protected abstract void RunProceduralGeneration();

    public abstract void Clear();
}
