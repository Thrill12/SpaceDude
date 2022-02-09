using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemLockedInteractable : InteractableObject
{
    public BaseItem itemRequired;
    public bool doesUseUpItem = false;

    public override void Interact()
    {
        if(UIManager.instance.inv.playerInventoryItems.Any(x => x.itemName == itemRequired.itemName))
        {
            if (doesUseUpItem)
            {
                UIManager.instance.inv.playerInventoryItems.First(x => x.itemName == itemRequired.itemName).itemStack -= 1;
            }

            base.Interact();
        }
        else
        {
            Debug.Log("Player does not have " + itemRequired.itemName);
        }
    }
}
