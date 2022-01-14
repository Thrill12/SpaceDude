using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public static Inventory instance;

    
    public BaseEntity player;
    
    public WeaponsHolder weaponHolder;

    public GameObject itemInventoryDisplay;
    public GameObject uiItemHolder;
    public GameObject worldItemHolder;
    public GameObject slotsParent;

    [Space(10)]

    public List<ItemSlot> slots = new List<ItemSlot>();

    [Space(10)]

    public List<BaseItem> items = new List<BaseItem>();

    [Space(10)]

    public List<BaseItem> itemsEquipped = new List<BaseItem>();

    [HideInInspector]
    public List<UIItemHolder> uiItemHolders = new List<UIItemHolder>();

    public int maxItemCount = 30;

    private void Awake()
    {
        instance = this;
    }

    //Update used to detect when the user clicks on an item holder in the game world
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePoss = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePoss2D = new Vector2(mousePoss.x, mousePoss.y);

            RaycastHit2D hitt = Physics2D.Raycast(mousePoss2D, Vector2.zero);

            if (hitt != false)
            {
                if (hitt.collider.CompareTag("Item"))
                {
                    hitt.collider.gameObject.GetComponent<ItemHolder>().ClickedOn(gameObject);
                }
            }
        }
    }

    public void SpawnDroppedItem(BaseItem itemDropped)
    {
        RemoveItemFromInventory(itemDropped);
        GameObject i = Instantiate(worldItemHolder, transform.position, Quaternion.identity);
        i.GetComponent<ItemHolder>().itemHeld = itemDropped;
    }

    public bool AddItem(BaseItem itemToAdd)
    {
        if (items.Count < maxItemCount)
        {
            if (!typeof(BaseEquippable).IsAssignableFrom(itemToAdd.GetType()))
            {
                if (items.Where(x => x.itemName == itemToAdd.itemName).Count() > 0)
                {
                    GeneralItem item = (GeneralItem)items.Where(x => x.itemName == itemToAdd.itemName).First();
                    GeneralItem genItemToAdd = (GeneralItem)itemToAdd;
                    item.itemStack += genItemToAdd.itemStack;
                }
                else
                {
                    items.Add(itemToAdd);
                    GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
                    uiHolder.GetComponent<UIItemHolder>().itemHeld = itemToAdd;
                    uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());
                }
            }
            else
            {
                items.Add(itemToAdd);
                GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
                uiHolder.GetComponent<UIItemHolder>().itemHeld = itemToAdd;
                uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());
            }

            return true;
        }
        else
        {
            Debug.Log("Inventory Full");
            return false;
        }
    }

    public void RemoveItemFromInventory(BaseItem itemToRemove)
    {
        //If we are trying to drop an equipped item, we need to also unequip it first so it
        //doesn't mess up with other stuff
        if (typeof(BaseEquippable).IsAssignableFrom(itemToRemove.GetType()))
        {
            BaseEquippable equippable = (BaseEquippable)itemToRemove;
            if (equippable.isEquipped)
            {
                UnequipItem(equippable);
            }
        }

        items.Remove(itemToRemove);
        uiItemHolders.Remove(uiItemHolders.Where(x => x.itemHeld == itemToRemove).First());
    }

    //These 2 functions handle equipping and unequipping items, excluding weapons which are handled below.
    public void EquipItem(BaseEquippable itemToEquip)
    {
        Debug.Log("Equipping item");

        itemToEquip.OnEquip(player);
        itemsEquipped.Add(itemToEquip);
        items.Remove(itemToEquip);

        ItemSlot slot = slots.Where(x => x.slotName == itemToEquip.itemSlot).First();

        if (slot.itemInSlot != null)
        {
            UnequipItem(slot.itemInSlot);

            //Checks if the item is a weapon
            if (typeof(BaseWeapon).IsAssignableFrom(itemToEquip.GetType()))
            {
                weaponHolder.UnequipWeapon((BaseWeapon)slot.itemInSlot);
            }
        }

        slot.itemInSlot = itemToEquip;

        Debug.Log("Placed in slot " + slot.slotName);

        //Checks if the item is a weapon
        if (typeof(BaseWeapon).IsAssignableFrom(itemToEquip.GetType()))
        {
            BaseWeapon weap = (BaseWeapon)itemToEquip;
            weaponHolder.EquipWeapon(weap);
            weaponHolder.SwapWeapons();
        }

        //Don't delete this "if loop" becasue i dont know why it doesnt work without it
        if (slot.itemInSlot == itemToEquip)
        {

        }
        else
        {
            EquipItem(itemToEquip);
        }
        itemToEquip.isEquipped = true;
        itemToEquip.hostEntity = player;

        Debug.Log("Reached the end");
    }

    public void UnequipItem(BaseEquippable itemToUnequip)
    {
        Debug.Log("Unequipping");
        itemToUnequip.OnUnequip();
        itemsEquipped.Remove(itemToUnequip);
        items.Add(itemToUnequip);

        ItemSlot slot = slots.Where(x => x.slotName == itemToUnequip.itemSlot).First();
        slot.itemInSlot = null;

        if (typeof(BaseWeapon).IsAssignableFrom(itemToUnequip.GetType()))
        {
            weaponHolder.UnequipWeapon((BaseWeapon)itemToUnequip);
            weaponHolder.StowWeaponWhenUnequipping((BaseWeapon)itemToUnequip);
        }

        //Don't delete this "if loop" becasue i dont know why it doesnt work without it
        if (slot.itemInSlot == null)
        {

        }
        else
        {
            UnequipItem(itemToUnequip);
        }

        itemToUnequip.isEquipped = false;
        Debug.Log(itemToUnequip.isEquipped);
    }
}
