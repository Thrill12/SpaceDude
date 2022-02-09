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

        Sprite spr = Sprite.Create(gen.GenerateForDebugging(), new Rect(0.0f, 0.0f, profile.gridSize, profile.gridSize), new Vector2(0.5f, 0.5f), 1);
        image.sprite = spr;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

