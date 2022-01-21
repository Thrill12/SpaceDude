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
    public int itemStack;
    public int itemMaxStack = 1;
    public ItemType itemType;
}

public enum ItemType
{
    None,
    Food,
    Electronic,
    BuildingSupply,
    Breathing,
    Waste,
    Weapon,
    Ammo,
    Raw
}