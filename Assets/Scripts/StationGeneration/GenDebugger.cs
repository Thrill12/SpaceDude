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

        HashSet<TriangulationGraph.Edge> graph = new TriangulationGraph().DelenauyTriangulation(gen.RoomPositions(), profile.gridSize);
        List<Vector3> pos = new List<Vector3>();

        foreach (TriangulationGraph.Edge e in graph)
        {
            pos.Add(e.point1 - new Vector2(profile.gridSize/2, profile.gridSize/2));
            pos.Add(e.point2 - new Vector2(profile.gridSize/2, profile.gridSize/2));
        }

        LineRenderer lr = gameObject.AddComponent<LineRenderer>();
        lr.positionCount = (graph.Count * 2);
        lr.SetPositions(pos.ToArray());

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
