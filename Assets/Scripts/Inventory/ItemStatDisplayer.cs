using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatDisplayer : MonoBehaviour
{
    public Image itemIcon;
    public Image itemBorder;
    public TMP_Text itemName;
    public TMP_Text itemRarity;
    public TMP_Text itemValue;
    public TMP_Text itemDescription;

    [Header("Stats")]
    public GameObject itemStatObject;
    public GameObject itemStatsHolder;   

    private List<GameObject> activatedStats = new List<GameObject>();

    public void ShowItem(BaseItem item)
    {
        if(item as BaseEquippable)
        {
            BaseEquippable equip = item as BaseEquippable;

            foreach (var statDisplay in activatedStats)
            {
                Destroy(statDisplay.gameObject);
            }

            foreach (var stat in equip.itemMods)
            {
                GameObject statObj = Instantiate(itemStatObject, itemStatsHolder.transform);
                TMP_Text statText = statObj.GetComponent<TMP_Text>();

                TextInfo info = new CultureInfo("en-US", false).TextInfo;

                if (stat.Type == Modifier.StatModType.Flat)
                {
                    statText.text = "- + " + Mathf.RoundToInt(stat.Value) + " " + info.ToTitleCase(stat.statDisplayStringName) + ".";
                }
                else if (stat.Type == Modifier.StatModType.PercentAdd)
                {
                    statText.text = "- + " + Mathf.RoundToInt(stat.Value) + "% " + info.ToTitleCase(stat.statDisplayStringName) + ".";
                }
                else if (stat.Type == Modifier.StatModType.PercentMult)
                {
                    statText.text = "- " + Mathf.RoundToInt(stat.Value) + "% increased " + info.ToTitleCase(stat.statDisplayStringName) + ".";
                }

                activatedStats.Add(statObj);
            }
        }

        itemIcon.sprite = item.itemIcon;
        SetBorder(item);
        itemName.text = item.itemName;
        itemRarity.text = item.itemRarity.name;
        itemRarity.color = item.itemRarity.rarityColor;
        itemValue.text = "C:" + item.itemValue;
        itemDescription.text = item.itemDescription;               
    }

    public void SetBorder(BaseItem item)
    {
        if(item.itemRarity.name == "Common")
        {
            itemBorder.sprite = PrefabManager.instance.commonItemBorder;
        }
        else if (item.itemRarity.name == "Rare")
        {
            itemBorder.sprite = PrefabManager.instance.rareItemBorder;
        }
        else if(item.itemRarity.name == "Royal")
        {
            itemBorder.sprite = PrefabManager.instance.royalItemBorder;
        }
        else
        {
            itemBorder.sprite = PrefabManager.instance.ascendedItemBorder;
        }
    }
}
