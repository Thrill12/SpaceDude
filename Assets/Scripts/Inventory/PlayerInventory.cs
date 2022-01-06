using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName ="Inventory/Player Inventory")]
public class PlayerInventory : Inventory
{
    public BaseEntity playerEntity;
    public WeaponsHolder weaponHolder;

    [Space(5)]

    public GameObject worldItemHolder;

    [Space(5)]

    public List<Inventory> itemSlots;

    public void EquipItem(BaseEquippable itemToEquip)
    {
        Inventory slotToEquipIn = itemSlots.Find(x => x.inventoryName == itemToEquip.itemSlot);

        if (slotToEquipIn.inventoryName != itemToEquip.itemSlot) return;

        if (slotToEquipIn.AddItem(itemToEquip))
        {            
            if(typeof(BaseWeapon).IsAssignableFrom(itemToEquip.GetType()))
            {
                weaponHolder.EquipWeapon((BaseWeapon)itemToEquip);
                weaponHolder.SwapWeapons();
                itemToEquip.isEquipped = true;
                RemoveItem(items.Find(x => x.item == itemToEquip));
            }
            else
            {
                itemToEquip.isEquipped = true;
                itemToEquip.OnEquip(playerEntity);
                RemoveItem(items.Find(x => x.item == itemToEquip));
            }
        }
        else
        {
            UnequipItem(slotToEquipIn.items.First().item as BaseEquippable);

            if (typeof(BaseWeapon).IsAssignableFrom(slotToEquipIn.items.First().item.GetType()))
            {
                weaponHolder.UnequipWeapon((BaseWeapon)slotToEquipIn.items.First().item);
            }

            EquipItem(itemToEquip);
        }
    }  

    public void UnequipItem(BaseEquippable itemToUnequip)
    {
        Inventory slotToUnequipFrom = itemSlots.Find(x => x.inventoryName == itemToUnequip.itemSlot);

        StoredItem stItem = slotToUnequipFrom.items.Find(x => x.item == itemToUnequip);

        if (typeof(BaseWeapon).IsAssignableFrom(itemToUnequip.GetType()))
        {
            weaponHolder.UnequipWeapon((BaseWeapon)itemToUnequip);
        }

        slotToUnequipFrom.RemoveItem(stItem);
        AddItem(itemToUnequip);
        itemToUnequip.OnUnequip();
        itemToUnequip.isEquipped = false;
    }

    public void SpawnDroppedItem(BaseItem itemDropped)
    {
        if(items.Find(x => x.item == itemDropped) == null)
        {
            foreach (var item in itemSlots)
            {
                if(item.items.Find(x => x.item == itemDropped) != null)
                {
                    RemoveItem(item.items.Find(x => x.item == itemDropped));
                }
            }
        }

        GameObject worldItem = Instantiate(worldItemHolder, Input.mousePosition, Quaternion.identity);
        worldItem.GetComponent<ItemHolder>().itemHeld = itemDropped;
    }
}
