using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [HideInInspector]
    public BaseEntity player;
    [HideInInspector]
    public WeaponsHolder weaponHolder;

    public GameObject itemDisplay;
    public GameObject uiItemHolder;
    public GameObject worldItemHolder;

    [Space(10)]

    public List<ItemSlot> slots = new List<ItemSlot>();

    [Space(10)]

    public List<BaseItem> items = new List<BaseItem>();

    [Space(10)]

    public List<BaseItem> itemsEquipped = new List<BaseItem>();

    [HideInInspector]
    public List<UIItemHolder> uiItemHolders = new List<UIItemHolder>();

    public int maxItemCount = 30;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<BaseEntity>();
        weaponHolder = GetComponent<WeaponsHolder>();
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

    //This drops an item from the players innventory at the player's position
    public void SpawnDroppedItem(BaseItem itemDropped)
    {
        RemoveItemFromInventory(itemDropped);
        GameObject i = Instantiate(worldItemHolder, transform.position, Quaternion.identity);
        i.GetComponent<ItemHolder>().itemHeld = itemDropped;
    }

    //Adding the item to the inventory and also adding the UI holder for it in the player's inventory
    public void AddItemToInventory(BaseItem itemToAdd)
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
                    GameObject uiHolder = Instantiate(uiItemHolder, itemDisplay.transform);
                    uiHolder.GetComponentInChildren<UIItemHolder>().itemHeld = itemToAdd;
                    uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());
                }
            }
            else
            {
                items.Add(itemToAdd);
                GameObject uiHolder = Instantiate(uiItemHolder, itemDisplay.transform);
                uiHolder.GetComponent<UIItemHolder>().itemHeld = itemToAdd;
                uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());
            }
        }
        else
        {
            Debug.Log("Inventory Full");
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


        //Checks if the item is a weapon
        if (typeof(BaseWeapon).IsAssignableFrom(itemToEquip.GetType()))
        {
            weaponHolder.EquipWeapon((BaseWeapon)itemToEquip);
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
        itemToEquip.hostEntity = GetComponent<BaseEntity>();
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
        itemToUnequip.hostEntity = null;
        Debug.Log(itemToUnequip.isEquipped);
    }
}
