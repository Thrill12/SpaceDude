using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemLockedInteractable : InteractableObject
{
    public BaseItem itemRequired;
    public bool doesUseUpItem = false;

    private PlayerInventory inventory;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
    }

    public override void Interact()
    {
        if(inventory.playerInventoryItems.Any(x => x.itemName == itemRequired.itemName))
        {
            if (doesUseUpItem)
            {
                inventory.playerInventoryItems.First(x => x.itemName == itemRequired.itemName).itemStack -= 1;
            }

            base.Interact();
        }
        else
        {
            Debug.Log("Player does not have " + itemRequired.itemName);
        }
    }
}
