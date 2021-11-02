using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Commodity : ScriptableObject
{
    public Sprite commodityIcon;
    public string commodityName;
    public int commodityPrice;
    public float stack;
    public Type commodityType;

    public enum Type
    {
        Food,
        Electronic,
        BuildingSupply,
        Breathing,
        Waste
    }
}
