using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Modifier
{
    //Name of the stat to change - it has to be the exact same as the IN-CODE property name, eg maxHealth
    public string statStringName;
    [Tooltip("Display name that the player will see in-game")]
    public string statDisplayStringName;
    public float Value;
    //The type of addition that the mod will do to the stat value
    public StatModType Type;
    //The stat being affected by the modifier = null by default
    public Stat statAffecting;
    [HideInInspector]
    public int Order;
    public BaseItem Source; // Added this variable

    // "Main" constructor. Requires all variables.
    public Modifier(string statDisplayStringName, Stat statToAffect, float value, StatModType type, int order, BaseItem source) // Added "source" input parameter
    {
        statAffecting = statToAffect;
        this.statDisplayStringName = statDisplayStringName;
        Value = value;
        Type = type;
        Order = order;
        Source = source; // Assign Source to our new input parameter      
    }

    // Requires Stat, Value and Type. Calls the "Main" constructor and sets Order and Source to their default values: (int)type and null, respectively.
    public Modifier(string statDisplayStringName, Stat statToAffect, float value, StatModType type) : this(statDisplayStringName, statToAffect, value, type, (int)type, null) { }

    // Requires Stat, Value, Type and Order. Sets Source to its default value: null
    public Modifier(string statDisplayStringName, Stat statToAffect, float value, StatModType type, int order) : this(statDisplayStringName, statToAffect, value, type, order, null) { }

    // Requires Stat, Value, Type and Source. Sets Order to its default value: (int)Type
    public Modifier(string statDisplayStringName, Stat statToAffect, float value, StatModType type, BaseItem source) : this(statDisplayStringName, statToAffect, value, type, (int)type, source) { }

    public enum StatModType
    {
        Flat = 100,
        PercentMult = 200,
        PercentAdd = 300,
    }
}
