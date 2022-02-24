using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BaseItem itemHeld;
    private Inventory inventory;

    private GameObject canvasObject;

    private Image img;
    private TMP_Text itemNameText;

    private bool isHoveredOn;

    private void Awake()
    {
        canvasObject = GameObject.FindGameObjectWithTag("UICanvas");
        
        img = GetComponentsInChildren<Image>()[1];
    }

    private void Start()
    {
        inventory = Inventory.instance;
        
        if (itemHeld.itemIcon != null)
        {
            img.sprite = itemHeld.itemIcon;
        }
        else
        {
            img.color = itemHeld.itemRarity.rarityColor;
        }

        if(itemHeld.itemRarity.name == "Common")
        {
            GetComponentInChildren<Image>().sprite = PrefabManager.instance.commonItemBorder;
        }
        else if(itemHeld.itemRarity.name == "Rare")
        {
            GetComponentInChildren<Image>().sprite = PrefabManager.instance.rareItemBorder;
        }
        else if(itemHeld.itemRarity.name == "Royal")
        {
            GetComponentInChildren<Image>().sprite = PrefabManager.instance.royalItemBorder;
        }
        else
        {
            GetComponentInChildren<Image>().sprite = PrefabManager.instance.ascendedItemBorder;
        }
    }

    private void Update()
    {      
        if (inventory.shipInventoryItems.Contains(itemHeld))
        {
            transform.SetParent(inventory.shipItemInventoryDisplay.transform);
        }
        else
        {
            CheckingBeingEquippedOrNot();
        }

        if (UIManager.instance.playerInput.currentControlScheme == "GamePad")
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                isHoveredOn = true;
                UIManager.instance.currentlySelectedItemToDisplay = itemHeld;
            }
            else
            {
                isHoveredOn = false;
            }
        }

        if (isHoveredOn)
        {
            UIManager.instance.currentlySelectedItemToDisplay = itemHeld;
        }       
    }

    private void CheckingBeingEquippedOrNot()
    {
        //This checks whether the item held derives from a general item or an equippable item
        //No need to do anything else if the item is not equippable, so no else though this might change
        //if we add more use cases for general items
        if (typeof(BaseEquippable).IsAssignableFrom(itemHeld.GetType()))
        {
            BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;

            if (eqItemHeld.isEquipped)
            {
                transform.SetParent(inventory.slotsParent.transform.Find(eqItemHeld.itemSlot).transform);
                Debug.Log("Setting for equipped");
            }
            else
            {
                transform.SetParent(inventory.itemInventoryDisplay.transform);
                Debug.Log("Setting for not equipped");
            }
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        isHoveredOn = true;
        UIManager.instance.SelectUIObject(gameObject);
    }

    public void OnPointerExit(PointerEventData data)
    {
        isHoveredOn = false;
        UIManager.instance.SelectUIObject(null);
        UIManager.instance.currentlySelectedItemToDisplay = null;
    }

    private void OnDisable()
    {
        isHoveredOn = false;
        UIManager.instance.SelectUIObject();
    }

    public void ToggleEquipUnequip()
    {
        if (!UIManager.instance.isInShipInventory)
        {
            if(itemHeld as BaseEquippable)
            {
                BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;

                if (eqItemHeld.isEquipped)
                {
                    Unequip();
                }
                else
                {
                    Equip();
                }
            }            
        }
        else
        {
            inventory.SwapInventoryOfItem(itemHeld);
        }
    }

    //We only use these functions when we are trying to equip an item, and we are already checking if item held is
    //equippable in update, no need to check here again.
    public void Equip()
    {
        BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;
        eqItemHeld.isEquipped = true;
        Inventory.instance.EquipItem(eqItemHeld);
        UIManager.instance.SelectFirstItemHolder();
        Debug.Log("Equipped item " + itemHeld.itemName);
    }

    public void Unequip()
    {
        BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;
        eqItemHeld.isEquipped = false;
        Inventory.instance.UnequipItem(eqItemHeld);
    }
}
