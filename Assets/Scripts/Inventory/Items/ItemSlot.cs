using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Item Slot"), System.Serializable]
public class ItemSlot : ScriptableObject
{
    public string slotName;
    public BaseEquippable itemInSlot;

    public void AddToSlot(BaseEquippable itemToAdd)
    {
        itemInSlot = itemToAdd;
    }
}
