using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemDrop
{
    //Small script to place in a drop table, to enable items to have weights as
    // I don't want that to be on the item itself - different enemies might have different drop rates for the same gun
    public BaseItem item;
    public int weight;
}
