using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class ItemHolder : MonoBehaviour
{
    public BaseItem itemHeld;
    public bool generateStats = false;
    public int stack = 1;
    private SpriteRenderer spriteRenderer;
    private PrefabManager prefabManager;
    private PlayerInventory playerInventory;
    private UIManager uiManager;

    private void Start()
    {
        playerInventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        playerInventory.LoadResourcesForItem(itemHeld);

        prefabManager = PrefabManager.instance;
        if(itemHeld != null)
        {
            itemHeld = ScriptableObject.Instantiate(itemHeld);
        }
        else
        {
            Destroy(gameObject);
        }

        if (stack <= itemHeld.itemMaxStack)
        {
            itemHeld.itemStack = stack;
        }
        else
        {
            itemHeld.itemStack = itemHeld.itemMaxStack;
        }
        
        //Setting up the item holder on the floor, its light/color etc.
        GetComponentInChildren<Light2D>().color = itemHeld.itemRarity.rarityColor;
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.color = itemHeld.itemRarity.rarityColor;

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
            equippable.hostEntity = uiManager.playerEntity;

            //Only generate random mods if the item is flagged for it
            if (generateStats && !(equippable as BaseWeapon))
            {
                equippable.GenerateMods();
            }
        }
    }

    public void ClickedOn()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("PlayerSuit"))
        {
            if (playerInventory.AddItem(itemHeld))
            {
                ClickedOn();
            }
        }
    }
}
