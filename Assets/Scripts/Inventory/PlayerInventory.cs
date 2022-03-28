using FullSerializer;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{  
    public PlayerEntity player;
    public int playerCredits = 0;
    public int playerPower = 0;
    public TMP_Text playerCreditsText;
    public TMP_Text playerPowerText;

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
    public List<BaseEquippable> itemsEquipped = new List<BaseEquippable>();

    [Space(10)]
    
    public List<UIItemHolder> uiItemHolders = new List<UIItemHolder>();
    public List<UIItemHolder> shipUiItemHolders = new List<UIItemHolder>();

    public int maxPlayerItemCount = 30;
    public int maxShipItemCount = 30;

    [fsIgnore]
    public bool isInShipInventory;

    private UIManager uiManager;
    private GameEvents gameEvents;

    private void Start()
    {
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        gameEvents = GameObject.FindGameObjectWithTag("GameEvents").GetComponent<GameEvents>();

        playerInventoryItems.Clear();
        shipInventoryItems.Clear();
        itemsEquipped.Clear();
        uiItemHolders.Clear();
        shipUiItemHolders.Clear();

        foreach (var item in slots)
        {
            Destroy(item.itemInSlot);
            item.itemInSlot = null;
        }

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
        playerCredits = GameManager.instance.progressSave.inventorySave.playerCredits;

        for (int i = 0; i < GameManager.instance.progressSave.inventorySave.shipInventoryItems.Count(); i++)
        {
            BaseItem item = GameManager.instance.progressSave.inventorySave.shipInventoryItems[i];
            LoadResourcesForItem(item);
            //SetItemModifierSources(item);

            AddItemIgnoreLimits(item, shipInventoryItems, maxShipItemCount, shipUiItemHolders);
            Debug.Log(playerInventoryItems.Count() + " player items started in ship items");
            //SwapInventoryOfItem(item);
        }

        for (int i = 0; i < GameManager.instance.progressSave.inventorySave.playerInventoryItems.Count(); i++)
        {
            BaseItem item = GameManager.instance.progressSave.inventorySave.playerInventoryItems[i];

            LoadResourcesForItem(item);
            //SetItemModifierSources(item);
            if (item as BaseEquippable)
            {
                BaseEquippable equip = item as BaseEquippable;
                if (!equip.isEquipped)
                {
                    AddItemIgnoreLimits(item, playerInventoryItems, maxPlayerItemCount, uiItemHolders);
                }
                else
                {
                    Destroy(equip);
                }
            }
            else
            {
                AddItemIgnoreLimits(item, playerInventoryItems, maxPlayerItemCount, uiItemHolders);
            }
        }

        for (int i = 0; i < GameManager.instance.progressSave.inventorySave.itemsEquipped.Count(); i++)
        {
            BaseEquippable equip = GameManager.instance.progressSave.inventorySave.itemsEquipped[i] as BaseEquippable;

            LoadResourcesForItem(equip);
            EquipItem(equip);

            if (equip as BaseWeapon)
            {
                weaponHolder.SwapWeapons();
            }

            if (playerInventoryItems.Contains(equip))
            {
                playerInventoryItems.Remove(equip);
            }

            Debug.Log(playerInventoryItems.Count() + " player items started in equipped items");
        }               
    }

    //Function to get the files required for hte item to enable them to be saved
    public void LoadResourcesForItem(BaseItem item)
    {
        GameManager.instance.LoadResourcesForItem(item);
    }

    //private static void SetItemModifierSources(BaseItem item)
    //{
    //    if (item as BaseEquippable)
    //    {
    //        BaseEquippable equip = item as BaseEquippable;
    //        foreach (var mod in equip.itemMods)
    //        {
    //            mod.Source = equip;
    //        }
    //    }
    //}

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
            return AddItemIgnoreLimits(itemToAdd, playerInventoryItems, maxPlayerItemCount, uiItemHolders);
        }
        else
        {
            Debug.Log("Inventory Full");
            return false;
        }       
    }

    private bool AddItemIgnoreLimits(BaseItem itemToAdd, List<BaseItem> inventoryList, int maxInInv, List<UIItemHolder> uiHolders)
    {
        gameEvents.OnItemPickedUp(itemToAdd);
        #region old
        //if (!typeof(BaseEquippable).IsAssignableFrom(itemToAdd.GetType()))
        //{
        //    if (playerInventoryItems.Where(x => x.itemName == itemToAdd.itemName).Count() > 0 && playerInventoryItems.Where(x => x.itemName == itemToAdd.itemName).Any(x => x.itemStack < x.itemMaxStack))
        //    {
        //        GeneralItem item = (GeneralItem)playerInventoryItems.Where(x => x.itemName == itemToAdd.itemName).Where(x => x.itemStack < x.itemMaxStack).First();
        //        GeneralItem genItemToAdd = (GeneralItem)itemToAdd;
        //        item.itemStack += genItemToAdd.itemStack;
        //    }
        //    else
        //    {
        //        playerInventoryItems.Add(itemToAdd);

        //        GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
        //        uiHolder.GetComponent<UIItemHolder>().itemHeld = itemToAdd;
        //        uiHolder.GetComponent<UIItemHolder>().SetBackground();
        //        uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());
        //    }
        //}
        //else
        //{
        //    playerInventoryItems.Add(itemToAdd);

        //    GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
        //    uiHolder.GetComponent<UIItemHolder>().itemHeld = itemToAdd;
        //    uiHolder.GetComponent<UIItemHolder>().SetBackground();
        //    uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());
        //}
        #endregion

        if (itemToAdd.itemMaxStack > 1)
        {
            //Code for stackable items

            if(inventoryList.Where(x => x.itemName == itemToAdd.itemName).Count() > 0 && inventoryList.Where(x => x.itemName == itemToAdd.itemName).Any(x => x.itemStack < x.itemMaxStack))
            {
                //Code if there is a stack of items already existing in the inventory with room to spare in the stack

                GeneralItem item = (GeneralItem)inventoryList.Where(x => x.itemName == itemToAdd.itemName).Where(x => x.itemStack < x.itemMaxStack).First();

                int remainingSpaceInItemStack = item.itemMaxStack - item.itemStack;

                if(remainingSpaceInItemStack > itemToAdd.itemStack)
                {
                    item.itemStack += itemToAdd.itemStack;
                    inventoryList.Remove(itemToAdd);
                    Destroy(itemToAdd);
                }
                else
                {
                    int excess = Mathf.Abs(remainingSpaceInItemStack);

                    item.itemStack = item.itemMaxStack;

                    itemToAdd.itemStack -= excess;

                    if(inventoryList.Count < maxInInv)
                    {
                        if (inventoryList.Where(x => x.itemName == itemToAdd.itemName).Count() > 0 && inventoryList.Where(x => x.itemName == itemToAdd.itemName).Any(x => x.itemStack < x.itemMaxStack))
                        {
                            AddItemIgnoreLimits(itemToAdd, inventoryList, maxInInv, uiHolders);
                        }
                        else
                        {
                            BaseItem newItem = Instantiate(itemToAdd);
                            inventoryList.Add(newItem);
                            Destroy(itemToAdd);

                            GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
                            uiHolder.GetComponent<UIItemHolder>().itemHeld = newItem;
                            uiHolder.GetComponent<UIItemHolder>().SetBackground();
                            uiHolders.Add(uiHolder.GetComponent<UIItemHolder>());
                        }                                                
                    }
                    else
                    {
                        Debug.Log("Inventory filled but added some bits");
                    }
                }
            }
            else
            {
                BaseItem newItem = Instantiate(itemToAdd);
                inventoryList.Add(newItem);
                Destroy(itemToAdd);

                GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
                uiHolder.GetComponent<UIItemHolder>().itemHeld = newItem;
                uiHolder.GetComponent<UIItemHolder>().SetBackground();
                uiHolders.Add(uiHolder.GetComponent<UIItemHolder>());
            }
        }
        else
        {
            BaseItem newItem = Instantiate(itemToAdd);
            inventoryList.Add(newItem);
            Destroy(itemToAdd);

            GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
            uiHolder.GetComponent<UIItemHolder>().itemHeld = newItem;
            uiHolder.GetComponent<UIItemHolder>().SetBackground();
            uiHolders.Add(uiHolder.GetComponent<UIItemHolder>());
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
            AddItemIgnoreLimits(itemToSwap, shipInventoryItems, maxShipItemCount, shipUiItemHolders);
            playerInventoryItems.Remove(itemToSwap);

            return true;
        }
        else if(shipInventoryItems.Contains(itemToSwap))
        {            
            AddItemIgnoreLimits(itemToSwap, playerInventoryItems, maxPlayerItemCount, uiItemHolders);
            shipInventoryItems.Remove(itemToSwap);

            return true;
        }
        else
        {
            return false;
        }
    }

    public void CalculatePlayerPower()
    {
        playerPower = itemsEquipped.Sum(x => x.itemLevel);
    }

    //These 2 functions handle equipping and unequipping items
    public void EquipItem(BaseEquippable itemToEquip)
    {
        ItemSlot slot = slots.Where(x => x.slotName == itemToEquip.itemSlot).First();

        if (itemToEquip != slot.itemInSlot && slot.itemInSlot != null)
        {
            UnequipItem(slot.itemInSlot);
        }

        itemToEquip.isEquipped = true;
        itemToEquip.hostEntity = player;
        playerInventoryItems.Remove(itemToEquip);
        itemToEquip.OnEquip(player);

        if (!uiItemHolders.Any(x => x.itemHeld == itemToEquip))
        {
            GameObject uiHolder = Instantiate(uiItemHolder, itemInventoryDisplay.transform);
            uiHolder.GetComponent<UIItemHolder>().itemHeld = itemToEquip;
            uiHolder.GetComponent<UIItemHolder>().SetBackground();
            uiItemHolders.Add(uiHolder.GetComponent<UIItemHolder>());
        }

        itemsEquipped.Add(itemToEquip);

        if (playerInventoryItems.Contains(itemToEquip))
        {
            playerInventoryItems.Remove(itemToEquip);
        }           

        if(uiManager.playerInput.currentActionMap.name == "GamePad")
        {
            uiManager.SelectFirstItemHolder();
        }

        slot.AddToSlot(itemToEquip);

        //Checks if the item is a weapon
        if (typeof(BaseWeapon).IsAssignableFrom(itemToEquip.GetType()))
        {
            BaseWeapon weap = (BaseWeapon)itemToEquip;
            weaponHolder.EquipWeapon(weap);
            weaponHolder.SwapWeapons();
        }

        player.CheckItemsEquippedSprites();
        CalculatePlayerPower();
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

        itemToUnequip.OnUnequip();
        itemToUnequip.isEquipped = false;

        player.CheckItemsEquippedSprites();
        CalculatePlayerPower();
    }
}
