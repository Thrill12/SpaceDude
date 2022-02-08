using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenDebugger : MonoBehaviour
{
    public GeneratorProfile profile;
    public int seed;
    public SpriteRenderer image;

    // Start is called before the first frame update
    public void Start()
    {
        Generator gen = new Generator(profile, seed);
        gen.GenerateRooms();

        TriangulationGraph tGraph = new TriangulationGraph();

        List<TriangulationGraph.Edge> graph = tGraph.DelenauyTriangulation(gen.RoomPositions(), profile.gridSize);

        List<TriangulationGraph.Edge> shortGraph = tGraph.MinimumSpanningTree(graph);
        

        foreach (TriangulationGraph.Edge e in graph)
        {
            GameObject go = new GameObject();
            go.transform.SetParent(this.transform);
            go.name = "Graph Edge";
            LineRenderer lr = go.AddComponent<LineRenderer>();

            lr.SetPosition(0, e.point1 - new Vector2(profile.gridSize / 2, profile.gridSize / 2));
            lr.SetPosition(1, e.point2 - new Vector2(profile.gridSize / 2, profile.gridSize / 2));
        }

        //Add corridors.
        int[,] mstTable = tGraph.GraphAdjacencyTable(shortGraph, tGraph.rooms, false);
        gen.GenerateCorridors(mstTable);

        Sprite spr = Sprite.Create(gen.GenerateDebugTexture(), new Rect(0.0f, 0.0f, profile.gridSize, profile.gridSize), new Vector2(0.5f, 0.5f), 1);
        image.sprite = spr;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

