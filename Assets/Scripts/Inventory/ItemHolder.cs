using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class ItemHolder : MonoBehaviour, IPointerClickHandler
{
    public BaseItem itemHeld;
    public bool generateStats = false;
    private TMP_Text itemNameText;
    private SpriteRenderer spriteRenderer;
    private PrefabManager prefabManager;
    private Inventory playerInventory;

    private void Start()
    {
        prefabManager = PrefabManager.instance;
        itemHeld = ScriptableObject.Instantiate(itemHeld);
        playerInventory = Inventory.instance;       

        //Setting up the item holder on the floor, its light/color etc.
        GetComponentInChildren<Light2D>().color = itemHeld.itemRarity.rarityColor;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = itemHeld.itemRarity.rarityColor;
        itemNameText = GetComponentInChildren<TMP_Text>();
        itemNameText.text = itemHeld.itemName;

        if (itemHeld.itemIcon != null)
        {
            spriteRenderer.sprite = itemHeld.itemIcon;
        }

        //This checks if the item is equippable, and lets it know it's not currently equipped as it has just spawned on the floor, in case unequipping it for some reason
        // doesn't clear its equipped flag.
        if (typeof(BaseEquippable).IsAssignableFrom(itemHeld.GetType()))
        {
            BaseEquippable equippable = (BaseEquippable)itemHeld;
            equippable.isEquipped = false;

            //Only generate random mods if the item is flagged for it
            if (generateStats)
            {
                equippable.GenerateMods();
            }
        }

        itemNameText.gameObject.SetActive(false);
    }

    public void ClickedOn(GameObject player)
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerSuit"))
        {
            itemNameText.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerSuit"))
        {
            itemNameText.gameObject.SetActive(false);
        }
    }

    public void OnPointerClick(PointerEventData data)
    {
        Inventory.instance.AddItem(itemHeld);
        ClickedOn(GameObject.FindGameObjectWithTag("PlayerSuit"));
    }
}
