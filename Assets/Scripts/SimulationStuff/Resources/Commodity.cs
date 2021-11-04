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

    public Commodity()
    {

    }

    public Commodity(Commodity comm, float stack)
    {
        commodityIcon = comm.commodityIcon;
        commodityName = comm.commodityName;
        commodityPrice = comm.commodityPrice;
        commodityType = comm.commodityType;
        this.stack = stack;
    }

    public enum Type
    {
        Food,
        Electronic,
        BuildingSupply,
        Breathing,
        Waste
    }
}
