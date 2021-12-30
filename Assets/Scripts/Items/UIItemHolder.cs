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
    private GameObject inventoryDisplayObject;
    private GameObject equippedInventoryDisplayObject;

    private Image img;
    private TMP_Text itemNameText;

    private bool isHoveredOn;

    private void Awake()
    {
        canvasObject = GameObject.FindGameObjectWithTag("UICanvas");
        inventory = GameObject.FindGameObjectWithTag("Player").GetComponent<Inventory>();
        inventoryDisplayObject = canvasObject.transform.Find("InventoryBackground").transform.Find("InventoryHolder").gameObject;
        equippedInventoryDisplayObject = canvasObject.transform.Find("EquippedItemsDisplay").transform.Find("InventoryHolder").gameObject;
        img = GetComponent<Image>();
        itemNameText = GetComponentInChildren<TMP_Text>();
    }

    private void Start()
    {
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

        itemNameText.text = itemHeld.itemName;
        canvasObject.GetComponent<Canvas>().worldCamera = Camera.main;
    }

    private void Update()
    {
        //This checks whether the item held derives from a general item or an equippable item
        //No need to do anything else if the item is not equippable, so no else though this might change
        //if we add more use cases for general items
        if (typeof(BaseEquippable).IsAssignableFrom(itemHeld.GetType()))
        {
            BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;

            if (isHoveredOn && Input.GetMouseButtonDown(1) && !eqItemHeld.isEquipped)
            {
                Equip();
            }
            else if (isHoveredOn && Input.GetMouseButtonDown(1) && eqItemHeld.isEquipped)
            {
                Unequip();
            }

            if (eqItemHeld.isEquipped)
            {
                transform.parent = equippedInventoryDisplayObject.transform;
            }
            else
            {
                transform.parent = inventoryDisplayObject.transform;
            }
        }

        if (isHoveredOn && Input.GetKeyDown(KeyCode.Z))
        {
            inventory.SpawnDroppedItem(itemHeld);
            Destroy(gameObject);
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

    //We only use these functions when we are trying to equip an item, and we are already checking if item held is
    //equippable in update, no need to check here again.
    public void Equip()
    {
        BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;
        eqItemHeld.isEquipped = true;
        inventory.EquipItem(eqItemHeld);
        Debug.Log(itemHeld.itemName + " equipped");
    }

    public void Unequip()
    {
        BaseEquippable eqItemHeld = (BaseEquippable)itemHeld;
        eqItemHeld.isEquipped = false;
        inventory.UnequipItem(eqItemHeld);
        Debug.Log(itemHeld.itemName + " unequipped");
    }
}
