using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseItem : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemIcon;
    public ItemRarity itemRarity;
    public int itemValue;
    public int itemStack = 1;
    public int itemMaxStack = 1;
    public ItemType itemType;
}

public enum ItemType
{
    None,
    Food,
    Electronic,
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