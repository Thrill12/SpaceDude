using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class PlanetProduction
{   
    public int comAmountPerTick;
    public bool lookingForTypeOnly = false;
    [ConditionalField("lookingForTypeOnly")]        public ItemType typeLookingFor;
    [ConditionalField("lookingForTypeOnly", true)]  public BaseItem comProduced;

    public PlanetProduction(BaseItem comm, int comTick)
    {
        comProduced = comm;
        comAmountPerTick = comTick;
    }

    public PlanetProduction(ItemType type, int comTick)
    {
        lookingForTypeOnly = true;
        typeLookingFor = type;
        comAmountPerTick = comTick;
    }
}
