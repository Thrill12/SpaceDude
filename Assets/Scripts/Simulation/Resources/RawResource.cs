using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Resource")]
public class RawResource : BaseItem
{
    [Header("Climate stats")]

    public ResourceClimateRequirement climateToGrow;
    public float resourceSpawnWeight;
}
