using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Rarity Table")]
public class RarityTable : ScriptableObject
{
    public List<BaseRarity> rarities;

    private static bool hasInit = false;
    private static float totalRarityWeight;

    //Counting total weight to figure out correct drop chances
    public void InitializeRarities()
    {
        if (hasInit) return;

        foreach (var item in rarities)
        {
            totalRarityWeight += item.rarityWeightChance;
        }
        hasInit = true;
    }

    //Weights are relative to each other, meaning that an item with rarity of 50 has double the chance of
    //getting picked than an item with rarity of 25.
    public BaseRarity GetRandomRarity()
    {
        InitializeRarities();
        float diceRoll = Random.Range(0, totalRarityWeight);

        foreach (var rarity in rarities)
        {
            if (rarity.rarityWeightChance >= diceRoll)
            {
                return rarity;
            }

            diceRoll -= rarity.rarityWeightChance;
        }

        throw new System.Exception("Rarity Generation Failed");
    }
}
