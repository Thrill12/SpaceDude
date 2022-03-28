using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PrefabManager : MonoBehaviour
{
    public static PrefabManager instance;

    public GameObject planetBorder;

    [Header("Sound Clips")]

    public AudioClip questAssignedSound;
    public AudioClip questGoalCompletedSound;
    public AudioClip explosionSound;

    [Header("UI")]

    public Color commonColour;
    public Color ascendedColour;
    public Color royalColour;
    public Color rareColour;

    [Space(5)]

    public GameObject planetHolderUI;
    public GameObject commodityMarketDisplayObject;
    public GameObject commodityInventoryDisplayObject;
    public GameObject galacticEventPrefab;

    [Space(5)]

    public Sprite commonItemBorder;
    public Sprite rareItemBorder;
    public Sprite royalItemBorder;
    public Sprite ascendedItemBorder;
    public Sprite unknownPersonPortrait;

    [Space(5)]

    public GameObject questDisplay;
    public GameObject questGoalDisplay;

    [Space(5)]

    public GameObject healthBarObject;

    [Space(10)]

    [Header("World UI")]

    public GameObject numberPopup;

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
            obj.GetComponent<ItemHolder>().stack = baseItem.itemStack;
            obj.GetComponent<ItemHolder>().itemHeld = baseItem;
            obj.GetComponent<ItemHolder>().generateStats = true;
        }
    }

    internal BaseRarity GetRandomRarity()
    {
        return rarityTable.GetRandomRarity();
    }

    //Spawns a number popup text in the world at desired location
    internal void SpawnNumberPopup(float number, Color colorPopup, Vector2 spawnPos)
    {
        GameObject num = Instantiate(numberPopup, spawnPos, Quaternion.identity);
        num.GetComponent<Canvas>().worldCamera = Camera.main;
        num.GetComponentInChildren<TMP_Text>().text = number.ToString();
        num.GetComponentInChildren<TMP_Text>().color = colorPopup;
        Destroy(num, 3);
    }
}
