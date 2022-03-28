using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIItemHolder : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BaseItem itemHeld;
    private PlayerInventory inventory;
    private UIManager uiManager;

    private GameObject canvasObject;

    public Image itemIcon;
    public TMP_Text itemStackText;
    public Image itemRarityDisplay;

    private bool isHoveredOn;

    private void Awake()
    {
        canvasObject = GameObject.FindGameObjectWithTag("UICanvas");
    }

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        if (itemHeld.itemIcon != null)
        {
            itemIcon.sprite = itemHeld.itemIcon;
        }
        else
        {
            itemIcon.color = itemHeld.itemRarity.rarityColor;
        }

        SetBackground();
    }

    public void SetBackground()
    {
        itemRarityDisplay.color = itemHeld.itemRarity.rarityColor;
    }

    private void Update()
    {
        if (itemHeld == null)
        { 
            if (inventory.uiItemHolders.Contains(this))
            {
                inventory.uiItemHolders.Remove(this);
            }
            else if (inventory.shipUiItemHolders.Contains(this))
            {
                inventory.shipUiItemHolders.Remove(this);
            }

            Destroy(gameObject);
        }

        if (inventory.shipInventoryItems.Contains(itemHeld))
        {
            transform.SetParent(inventory.shipItemInventoryDisplay.transform);
        }
        else
        {
            CheckingBeingEquippedOrNot();
        }

        if (uiManager.playerInput.currentControlScheme == "GamePad")
        {
            if (EventSystem.current.currentSelectedGameObject == gameObject)
            {
                isHoveredOn = true;
                uiManager.currentlySelectedItemToDisplay = itemHeld;
            }
            else
            {
                isHoveredOn = false;
            }
        }

        if (isHoveredOn && uiManager.currentlySelectedItemToDisplay != itemHeld)
        {
            uiManager.currentlySelectedItemToDisplay = itemHeld;
            uiManager.SelectUIObject(gameObject);
        }

        if(itemHeld.itemStack == 1)
        {
            itemStackText.text = "";
        }
        else
        {
            itemStackText.text = itemHeld.itemStack.ToString();
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
            }
            else
            {
                transform.SetParent(inventory.itemInventoryDisplay.transform);
            }
        }
        else
        {
            transform.SetParent(inventory.itemInventoryDisplay.transform);
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        isHoveredOn = true;
        uiManager.SelectUIObject(gameObject);
    }

    public void OnPointerExit(PointerEventData data)
    {
        isHoveredOn = false;
        uiManager.SelectUIObject(null);
        uiManager.currentlySelectedItemToDisplay = null;
    }

    private void OnDisable()
    {
        isHoveredOn = false;
        uiManager.SelectUIObject();
    }

    public void ToggleEquipUnequip()
    {
        if (!uiManager.isInShipInventory)
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
            else if(itemHeld as BaseConsumable)
            {
                BaseConsumable consumableItem = (BaseConsumable)itemHeld;

                consumableItem.Use();
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
        inventory.EquipItem(eqItemHeld);
        uiManager.SelectFirstItemHolder();
        uiManager.audioSource.PlayOneShot(uiManager.itemEquipSound);
        Debug.Log("Equipped item " + itemHeld.itemName);
    }

    public void Unequip()
    {
        BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;
        inventory.UnequipItem(eqItemHeld);
        uiManager.audioSource.PlayOneShot(uiManager.itemUnequipSound);
    }
}
