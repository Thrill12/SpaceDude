using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Modifier
{
    public string statToModifyName;
    public float Value;
    public StatModType Type;
    [HideInInspector]
    public int Order;
    public BaseItem Source; // Added this variable

    // "Main" constructor. Requires all variables.
    public Modifier(string stat, float value, StatModType type, int order, BaseItem source) // Added "source" input parameter
    {
        statToModifyName = stat;
        Value = value;
        Type = type;
        Order = order;
        Source = source; // Assign Source to our new input parameter
    }

    // Requires Stat, Value and Type. Calls the "Main" constructor and sets Order and Source to their default values: (int)type and null, respectively.
    public Modifier(string stat, float value, StatModType type) : this(stat, value, type, (int)type, null) { }

    // Requires Stat, Value, Type and Order. Sets Source to its default value: null
    public Modifier(string stat, float value, StatModType type, int order) : this(stat, value, type, order, null) { }

    // Requires Stat, Value, Type and Source. Sets Order to its default value: (int)Type
    public Modifier(string stat, float value, StatModType type, BaseItem source) : this(stat, value, type, (int)type, source) { }

    public enum StatModType
    {
        Flat = 100,
        PercentMult = 200,
        PercentAdd = 300,
    }
}
