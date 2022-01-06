using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ItemHolder : MonoBehaviour
{
    public BaseItem itemHeld;
    public bool generateStats = false;
    private TMP_Text itemNameText;
    private SpriteRenderer spriteRenderer;
    private PrefabManager prefabManager;

    private void Start()
    {
        prefabManager = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        itemHeld = ScriptableObject.Instantiate(itemHeld);

        if (generateStats)
        {
            GenerateMods();
        }


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
        }        
    }

    //Generates some basic, random mods for the item, depending on its rarity. Will include other factors later on
    // and i might move this bit of items (generating) into an item factory for ease of access, but not sure yet
    public void GenerateMods()
    {
        if(itemHeld.itemRarity.rarityName == "Common")
        {
            for (int i = 0; i < 2; i++)
            {
                GenerateMod();
            }
        }
        else if (itemHeld.itemRarity.rarityName == "Rare")
        {
            for (int i = 0; i < 3; i++)
            {
                GenerateMod();
            }
        }
        else if (itemHeld.itemRarity.rarityName == "Legendary")
        {
            for (int i = 0; i < 5; i++)
            {
                GenerateMod();
            }
        }
    }

    public void GenerateMod()
    {
        int randomProperty = Random.Range(0, 2);

        if (randomProperty == 0)
        {
            Modifier mod = new Modifier("damage", Random.Range(5, 50), Modifier.StatModType.Flat);
            mod.Source = itemHeld;
            BaseWeapon weapon = (BaseWeapon)itemHeld;
            if (weapon.damage.statModifiers.Where(x => x.Source == this).Count() == 0)
            {
                weapon.damage.AddModifier(mod);
            }
        }
        else if (randomProperty == 1)
        {
            Modifier mod = new Modifier("attackCooldown", Random.Range(-5, -50), Modifier.StatModType.PercentAdd);
            mod.Source = itemHeld;
            BaseWeapon weapon = (BaseWeapon)itemHeld;
            if (weapon.attackCooldown.statModifiers.Where(x => x.Source == this).Count() == 0)
            {
                weapon.attackCooldown.AddModifier(mod);
            }
        }
        else if (randomProperty == 2 && typeof(StraightShootingGun).IsAssignableFrom(itemHeld.GetType()))
        {
            Modifier mod = new Modifier("projectileSpeed", Random.Range(5, 50), Modifier.StatModType.PercentMult);
            mod.Source = itemHeld;
            StraightShootingGun weapon = (StraightShootingGun)itemHeld;
            if (weapon.projectileSpeed.statModifiers.Where(x => x.Source == this).Count() == 0)
            {
                weapon.projectileSpeed.AddModifier(mod);
            }
        }
    }

    private void Update()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

        RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
        if (hit.collider == gameObject.GetComponent<Collider2D>())
        {
            itemNameText.gameObject.SetActive(true);
        }
        else
        {
            itemNameText.gameObject.SetActive(false);
        }
    }

    public void ClickedOn(GameObject player)
    {
        if (player.GetComponent<Inventory>().items.Count < 30)
        {
            player.GetComponent<Inventory>().AddItemToInventory(itemHeld);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Inventory full");
        }
    }
}
