using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenDebugger : MonoBehaviour
{
    public GeneratorProfile profile;
    public int seed;
    public SpriteRenderer image;

    // Start is called before the first frame update
    void Start()
    {
        Generator gen = new Generator(profile, seed);
        Sprite spr = Sprite.Create(gen.GenerateWithDebugTexture(), new Rect(0.0f, 0.0f, profile.gridSize, profile.gridSize), new Vector2(0.5f, 0.5f), 1);
        image.sprite = spr;

        TriangulationGraph tGraph = new TriangulationGraph();

        List<TriangulationGraph.Edge> graph = tGraph.DelenauyTriangulation(gen.RoomPositions(), profile.gridSize);

        List<TriangulationGraph.Edge> shortGraph = tGraph.MinimumSoanningTree(graph);
        

        foreach (TriangulationGraph.Edge e in graph)
        {
            GameObject go = new GameObject();
            LineRenderer lr = go.AddComponent<LineRenderer>();

            lr.SetPosition(0, e.point1 - new Vector2(profile.gridSize / 2, profile.gridSize / 2));
            lr.SetPosition(1, e.point2 - new Vector2(profile.gridSize / 2, profile.gridSize / 2));
        }


    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

