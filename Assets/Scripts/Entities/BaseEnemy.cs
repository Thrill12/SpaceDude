using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BaseEnemy : BaseEntity
{
    public Image healthBar;
    public List<ItemDrop> dropTable;

    public float healthBarYOffset = 0.5f;

    private PrefabManager prefabManager;
    private GameObject healthBarObject;

    private static bool hasInit = false;
    private static float totalRarityWeight;

    public override void Start()
    {
        base.Start();

        prefabManager = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        healthBarObject = Instantiate(prefabManager.healthBarObject, transform);
        healthBarObject.transform.position = new Vector3(healthBarObject.transform.position.x, healthBarObject.transform.position.y + healthBarYOffset, healthBarObject.transform.position.z);
        healthBar = healthBarObject.transform.Find("HealthBar").GetComponent<Image>();
    }

    public override void Update()
    {
        base.Update();
        healthBar.fillAmount = health / maxHealth.Value;
    }

    public override void Die()
    {
        pfMan.SpawnItem(gameObject, GetRandomItem());
        base.Die();
    } 

    //Counting total weight to figure out correct drop chances
    public void InitializeItems()
    {
        if (hasInit) return;

        foreach (var item in dropTable)
        {
            totalRarityWeight += item.weight;
        }
        hasInit = true;
    }

    //Weights are relative to each other, meaning that an item with rarity of 50 has double the chance of
    //getting picked than an item with rarity of 25.
    public BaseItem GetRandomItem()
    {
        InitializeItems();

        float diceRoll = Random.Range(0, totalRarityWeight);

        foreach (var item in dropTable)
        {
            if (item.weight >= diceRoll)
            {
                return item.item;
            }

            diceRoll -= item.weight;
        }

        throw new System.Exception("Item Generation Failed");
    }
}