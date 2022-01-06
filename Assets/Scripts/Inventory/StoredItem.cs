using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class StoredItem
{
    public BaseItem item;
    public IntPair position;
    public Inventory inventory;

    public StoredItem()
    {

    }

    public StoredItem(BaseItem item, IntPair position)
    {
        this.item = item;
        this.position = position;
    }
}
