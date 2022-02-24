using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FullSerializer;

[System.Serializable]
public class BaseItem : ScriptableObject
{
    public string GUID;
    public string itemName;
    public string itemDescription;

    public Sprite itemIcon;
    [SerializeField, HideInInspector]
    public string itemIconPath;

    public ItemRarity itemRarity;
    public int itemValue;
    public int itemStack = 1;
    public int itemMaxStack = 1;
    public ItemType itemType;

    public BaseItem()
    {
        GenerateGUID();
    }

    private void GenerateGUID() => GUID = Guid.NewGuid().ToString();
}

public enum ItemType
{
    None,
    Food,
    Armour,
    Electronic,
    Aspite,
    Building_Supply,
    Breathing,
    Waste,
    Weapon,
    Kinetic_Clip,
    Shotgun_Shell,
    Kinetic_Belt,
    Spike_Ammo,
    Union_Clip,
    Raw
}