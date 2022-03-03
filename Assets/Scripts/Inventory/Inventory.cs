using FullSerializer;
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
    public GameObject shipItemInventoryDisplay;
    public GameObject uiItemHolder;
    public GameObject worldItemHolder;
    public GameObject slotsParent;

    [Space(10)]

    public List<ItemSlot> slots = new List<ItemSlot>();

    [Space(10)]

    public List<BaseItem> playerInventoryItems = new List<BaseItem>();
    public List<BaseItem> shipInventoryItems = new List<BaseItem>();

    [Space(10)]

    public List<BaseEquippable> itemsEquipped = new List<BaseEquippable>();

    public List<UIItemHolder> uiItemHolders = new List<UIItemHolder>();

    public List<UIItemHolder> shipUiItemHolders = new List<UIItemHolder>();

    public int maxPlayerItemCount = 30;
    public int maxShipItemCount = 30;

    [fsIgnore]
    public bool isInShipInventory;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        try
        {
            LoadItemsFromSave();
        }
        catch
        {
            Debug.Log("No save");
        }       
    }

    public void LoadItemsFromSave()
    {
        for (int i = 0; i < GameManager.instance.progressSave.inventorySave.shipInventoryItems.Count(); i++)
        {
            BaseItem item = GameManager.instance.progressSave.inventorySave.shipInventoryItems[i] as BaseItem;
            LoadResourcesForItem(item);
            SetItemModifierSources(item);

            AddItemIgnoreLimits(item);

            SwapInventoryOfItem(item);
        }

        for (int i = 0; i < GameManager.instance.progressSave.inventorySave.playerInventoryItems.Count(); i++)
        {
            BaseItem item = GameManager.instance.progressSave.inventorySave.playerInventoryItems[i] as BaseItem;
            LoadResourcesForItem(item);
            SetItemModifierSources(item);
            AddItemIgnoreLimits(item);
        }

        for (int i = 0; i < GameManager.instance.progressSave.inventorySave.itemsEquipped.Count(); i++)
        {
            BaseEquippable equip = Instantiate(GameManager.instance.progressSave.inventorySave.itemsEquipped[i] as BaseEquippable);
            LoadResourcesForItem(equip);
            SetItemModifierSources(equip);
            AddItemIgnoreLimits(equip);
            EquipItem(equip);

            if(equip as BaseWeapon)
            {
                weaponHolder.SwapWeapons();
            }

            equip.isEquipped = true;
            equip.hostEntity = player;

            //if(equip as BaseWeapon)
            //{
            //    weaponHolder.EquipWeapon(equip as BaseWeapon);
            //}

            //GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
            //uiHolder.GetComponent<UIItemHolder>().itemHeld = equip;
            //uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());            
        }
    }

    //Function to get the files required for hte item to enable them to be saved
    public void LoadResourcesForItem(BaseItem item)
    {
        GameManager.instance.LoadResourcesForItem(item);
    }

    private static void SetItemModifierSources(BaseItem item)
    {
        if (item as BaseEquippable)
        {
            BaseEquippable equip = item as BaseEquippable;
            foreach (var mod in equip.itemMods)
            {
                mod.Source = equip;
            }
        }
    }

    private void Update()
    {
        if (shipItemInventoryDisplay.activeInHierarchy)
        {
            isInShipInventory = true;
        }
        else
        {
            isInShipInventory = false;
        }

        for (int i = 0; i < uiItemHolders.Count; i++)
        {
            BaseItem item = uiItemHolders[i].itemHeld;
            if (item.itemStack <= 0)
            {
                RemoveItemFromInventory(item);
            }
        }
    }

    public void SpawnDroppedItem(BaseItem itemDropped)
    {
        RemoveItemFromInventory(itemDropped);
        GameObject i = Instantiate(worldItemHolder, transform.position, Quaternion.identity);
        i.GetComponent<ItemHolder>().itemHeld = itemDropped;
    }

    //Tries to add an item to the inventory - if inventory has item which can be stacked, it checks and adds to the
    //stack of the item
    public bool AddItem(BaseItem itemToAdd)
    {
        if (playerInventoryItems.Count < maxPlayerItemCount && !playerInventoryItems.Contains(itemToAdd))
        {
            return AddItemIgnoreLimits(itemToAdd);
        }
        else
        {
            Debug.Log("Inventory Full");
            return false;
        }       
    }

    private bool AddItemIgnoreLimits(BaseItem itemToAdd)
    {
        if (!typeof(BaseEquippable).IsAssignableFrom(itemToAdd.GetType()))
        {
            if (playerInventoryItems.Where(x => x.itemName == itemToAdd.itemName).Count() > 0)
            {
                GeneralItem item = (GeneralItem)playerInventoryItems.Where(x => x.itemName == itemToAdd.itemName).First();
                GeneralItem genItemToAdd = (GeneralItem)itemToAdd;
                item.itemStack += genItemToAdd.itemStack;
            }
            else
            {
                playerInventoryItems.Add(itemToAdd);

                GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
                uiHolder.GetComponent<UIItemHolder>().itemHeld = itemToAdd;
                uiHolder.GetComponent<UIItemHolder>().SetBackground();
                uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());
            }
        }
        else
        {
            playerInventoryItems.Add(itemToAdd);

            GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
            uiHolder.GetComponent<UIItemHolder>().itemHeld = itemToAdd;
            uiHolder.GetComponent<UIItemHolder>().SetBackground();
            uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());
        }
        return true;
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

        playerInventoryItems.Remove(itemToRemove);
        GameObject objToRemove = uiItemHolders.First(x => x.itemHeld == itemToRemove).gameObject;
        uiItemHolders.Remove(uiItemHolders.Where(x => x.itemHeld == itemToRemove).First());
        Destroy(objToRemove);
    }

    public bool SwapInventoryOfItem(BaseItem itemToSwap)
    {
        if (playerInventoryItems.Contains(itemToSwap))
        {
            //Run code to move from player to ship

            if (shipInventoryItems.Count >= maxShipItemCount) return false;

            playerInventoryItems.Remove(itemToSwap);
            shipInventoryItems.Add(itemToSwap);
            GameObject holder = uiItemHolders.Where(x => x.itemHeld == itemToSwap).First().gameObject;
            holder.transform.SetParent(shipItemInventoryDisplay.transform);
            uiItemHolders.Remove(holder.GetComponent<UIItemHolder>());
            shipUiItemHolders.Add(holder.GetComponent<UIItemHolder>());

            return true;
        }
        else
        {
            //Code to swap item from ship to player

            if (playerInventoryItems.Count >= maxPlayerItemCount) return false;

            shipInventoryItems.Remove(itemToSwap);
            playerInventoryItems.Add(itemToSwap);
            GameObject holder = shipUiItemHolders.Where(x => x.itemHeld == itemToSwap).First().gameObject;
            holder.transform.SetParent(itemInventoryDisplay.transform);
            shipUiItemHolders.Remove(holder.GetComponent<UIItemHolder>());
            uiItemHolders.Add(holder.GetComponent<UIItemHolder>());
            return true;
        }
    }

    //These 2 functions handle equipping and unequipping items, excluding weapons which are handled below.
    public void EquipItem(BaseEquippable itemToEquip)
    {
        itemToEquip.OnEquip(player);
        itemsEquipped.Add(itemToEquip);
        playerInventoryItems.Remove(itemToEquip);

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

        if(UIManager.instance.playerInput.currentActionMap.name == "GamePad")
        {
            UIManager.instance.SelectFirstItemHolder();
        }

        slot.itemInSlot = itemToEquip;

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
    }

    //Unequips item in the argument from the player
    public void UnequipItem(BaseEquippable itemToUnequip)
    {     
        itemsEquipped.Remove(itemToUnequip);
        playerInventoryItems.Add(itemToUnequip);

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

        itemToUnequip.OnUnequip();
        itemToUnequip.isEquipped = false;
    }
}
