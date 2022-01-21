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
        if (typeof(BaseEquippable).IsAssignableFrom(itemHeld.GetType()))
        {
            BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;
            eqItemHeld.isEquipped = false;
        }
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
        //This checks whether the item held derives from a general item or an equippable item
        //No need to do anything else if the item is not equippable, so no else though this might change
        //if we add more use cases for general items
        if (typeof(BaseEquippable).IsAssignableFrom(itemHeld.GetType()))
        {
            BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;

            Debug.Log(eqItemHeld.itemSlot);
            Debug.Log(inventory.slotsParent.transform.Find(eqItemHeld.itemSlot).gameObject.name);

            if (eqItemHeld.isEquipped)
            {
                transform.SetParent(inventory.slotsParent.transform.Find(eqItemHeld.itemSlot).transform);
            }
            else
            {
                transform.SetParent(inventory.itemInventoryDisplay.transform);
            }
        }

        if(EventSystem.current.currentSelectedGameObject == gameObject)
        {
            isHoveredOn = true;
        }
        else
        {
            isHoveredOn = false;
        }

        //Turn on the item stats display menu
        if (isHoveredOn)
        {
            if((itemHeld as BaseEquippable).isEquipped)
            {
                UIManager.instance.leftItemStatDisplay.gameObject.SetActive(true);
                UIManager.instance.rightItemStatDisplay.gameObject.SetActive(false);
                UIManager.instance.leftItemStatDisplay.ShowItem(itemHeld as BaseEquippable);                
            }
            else
            {
                UIManager.instance.leftItemStatDisplay.gameObject.SetActive(false);
                UIManager.instance.rightItemStatDisplay.gameObject.SetActive(true);
                UIManager.instance.rightItemStatDisplay.ShowItem(itemHeld as BaseEquippable);
            }
        }
        else
        {
            UIManager.instance.leftItemStatDisplay.gameObject.SetActive(false);
            UIManager.instance.leftItemStatDisplay.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData pointerEventData)
    {
        isHoveredOn = true;
        //Display Item Stats here
        //Show();
    }

    public void OnPointerExit(PointerEventData pointerEventData)
    {
        isHoveredOn = false;
        //Hide item stats here
        //Hide();
    }

    public void OnSelect(BaseEventData data)
    {
        isHoveredOn = true;

        Debug.Log("Selected");
    }

    public void OnDeselect(BaseEventData data)
    {
        isHoveredOn = false;        

        Debug.Log("Deselected");
    }

    public void ToggleEquipUnequip()
    {
        BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;

        if (eqItemHeld.isEquipped)
        {
            Unequip();
        }
        else
        {
            Equip(eqItemHeld.itemSlot);
        }
    }

    //We only use these functions when we are trying to equip an item, and we are already checking if item held is
    //equippable in update, no need to check here again.
    public void Equip(string slot)
    {
        BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;
        eqItemHeld.isEquipped = true;
        inventory.EquipItem(eqItemHeld);
        Debug.Log(itemHeld.itemName + " equipped");
        UIManager.instance.SelectFirstItemHolder();
    }

    public void Unequip()
    {
        BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;
        eqItemHeld.isEquipped = false;
        inventory.UnequipItem(eqItemHeld);
        Debug.Log(itemHeld.itemName + " unequipped");
    }
}
