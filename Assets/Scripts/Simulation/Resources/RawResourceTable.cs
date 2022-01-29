using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RawResourceTable : MonoBehaviour
{
    public static RawResourceTable instance;

    public List<RawResource> resources;

    private static bool hasInit = false;
    private static float totalResourceWeight;

    private void Awake()
    {
        instance = this;
    }

    //Counting total weight to figure out correct drop chances
    public void InitializeResources()
    {
        if (hasInit) return;

        foreach (var item in resources)
        {
            totalResourceWeight += item.resourceSpawnWeight;
        }
        hasInit = true;
    }

    //Weights are relative to each other, meaning that a resource with rarity of 50 has double the chance of
    //getting picked than a resource with rarity of 25.
    public RawResource GetRandomResources()
    {
        InitializeResources();
        float diceRoll = Random.Range(0, totalResourceWeight);

        foreach (var resource in resources)
        {
            if (resource.resourceSpawnWeight >= diceRoll)
            {
                return resource;
            }

            diceRoll -= resource.resourceSpawnWeight;
        }

        throw new System.Exception("Resource Generation Failed");
    }
}
