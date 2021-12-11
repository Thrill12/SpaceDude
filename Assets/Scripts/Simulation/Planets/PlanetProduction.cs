using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PlanetProduction
{   
    public float comAmountPerTick;
    public bool lookingForTypeOnly = false;
    [ConditionalField("lookingForTypeOnly")]        public Commodity.Type typeLookingFor;
    [ConditionalField("lookingForTypeOnly", true)]  public Commodity comProduced;

    public PlanetProduction(Commodity comm, float comTick)
    {
        comProduced = comm;
        comAmountPerTick = comTick;
    }

    public PlanetProduction(Commodity.Type type, float comTick)
    {
        lookingForTypeOnly = true;
        typeLookingFor = type;
        comAmountPerTick = comTick;
    }
}
