using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;

[System.Serializable]
public class Stat
{
    [SerializeField]
    public string statName;
    public float BaseValue = 1;
    //You want to access the value of the stat with .Value instead of _value, as that is a function
    //that returns the cleaned up version of the stat.
    //I exposed this to be easier to see in the inspector 
    [SerializeField, HideInInspector] public float _value;

    protected float lastBaseValue = float.MinValue;

    public List<Modifier> statModifiers;

    //This will tell us if we need to clean up the modifiers, as some addition types need to be added first before others
    [HideInInspector] public bool isDirty = true;

    public Stat()
    {
        statModifiers = new List<Modifier>();
    }

    public Stat(float baseValue) : this()
    {
        BaseValue = baseValue;
    }

    //This function cleans up any added or removed modifiers, and ensures only active modifiers are counted
    public float Value
    {
        get
        {
            if (isDirty || lastBaseValue != BaseValue)
            {
                lastBaseValue = BaseValue;
                _value = CalculateFinalValue();
                isDirty = false;
            }
            return _value;
        }
    }

    public virtual void AddModifier(Modifier mod)
    {
        isDirty = true;
        statModifiers.Add(mod);
        statModifiers.Sort(CompareModifierOrder);
        CalculateFinalValue();
    }

    //This method ensures that we use the correct order of addition types
    protected virtual int CompareModifierOrder(Modifier a, Modifier b)
    {
        if (a.Order < b.Order)
            return -1;
        else if (a.Order > b.Order)
            return 1;
        return 0; // if (a.Order == b.Order)
    }

    public virtual bool RemoveModifier(Modifier mod)
    {
        if (statModifiers.Remove(mod))
        {
            isDirty = true;
            return true;
        }
        return false;
    }

    //Used when getting the value of a stat to have it all cleaned up
    protected virtual float CalculateFinalValue()
    {
        float finalValue = BaseValue;
        float sumPercentAdd = 0; // This will hold the sum of our "PercentAdd" modifiers, as we have to add them all at the same time

        for (int i = 0; i < statModifiers.Count; i++)
        {
            Modifier mod = statModifiers[i];

            if (mod.Type == Modifier.StatModType.Flat)
            {
                finalValue += mod.Value;
            }
            else if (mod.Type == Modifier.StatModType.PercentAdd) // When we encounter a "PercentAdd" modifier
            {
                sumPercentAdd += mod.Value; // Start adding together all modifiers of this type

                // If we're at the end of the list OR the next modifer isn't of this type
                if (i + 1 >= statModifiers.Count || statModifiers[i + 1].Type != Modifier.StatModType.PercentAdd)
                {
                    finalValue += finalValue / 100 * sumPercentAdd; // Multiply the sum with the "finalValue", like we do for "PercentMult" modifiers
                    sumPercentAdd = 0; // Reset the sum back to 0
                }
            }
            else if (mod.Type == Modifier.StatModType.PercentMult) // Percent renamed to PercentMult
            {
                finalValue *= 1 + mod.Value / 100;
            }
        }

        return MathF.Round(finalValue, 1);
    }

    public virtual bool RemoveAllModifiersFromSource(object source)
    {
        bool didRemove = false;

        for (int i = statModifiers.Count - 1; i >= 0; i--)
        {
            if (statModifiers[i].Source == (object)source)
            {
                isDirty = true;
                didRemove = true;
                statModifiers.RemoveAt(i);
            }
        }

        return didRemove;
    }
}
