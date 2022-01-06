using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="Inventory/Slot")]
public class SlotInventory : Inventory
{
    public override bool AddItem(BaseItem item)
    {
        if (items.Count == 1) return false;

        if(item as BaseEquippable)
        {
            BaseEquippable equippable = (BaseEquippable)item;
            if(equippable.itemSlot == inventoryName)
            {
                return base.AddItem(item);
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }
}
