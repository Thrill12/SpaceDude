using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    public GameObject planetBorder;

    [Header("UI")]

    [Space(5)]

    public GameObject planetHolderUI;
    public GameObject commodityMarketDisplayObject;
    public GameObject commodityInventoryDisplayObject;
    public GameObject galacticEventPrefab;

    [Space(5)]

    public GameObject healthBarObject;

    [Space(5)]

    [Header("Objects")]

    public GameObject cargoShip;
    public GameObject itemObject;

    [Space(5)]

    public RarityTable rarityTable;

    [Space(5)]

    [Header("Player suit stuff")]

    public GameObject playerSuitBullet;
    public GameObject playerSuitYellowBulletImpactParticles;

    private void Awake()
    {
        instance = this;
    }

    //This is used by the inventory to spawn a dropped item
    internal void SpawnItem(GameObject objPos, BaseItem baseItem)
    {
        if(baseItem != null)
        {
            GameObject obj = Instantiate(itemObject, objPos.transform.position, Quaternion.identity);
            obj.GetComponent<ItemHolder>().itemHeld = baseItem;
            obj.GetComponent<ItemHolder>().generateStats = true;
        }
    }

    internal BaseRarity GetRandomRarity()
    {
        return rarityTable.GetRandomRarity();
    }
}
