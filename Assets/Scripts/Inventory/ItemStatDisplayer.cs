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
    public Image itemStatsBar;
    public Image itemDescriptionBorder;
    public TMP_Text itemName;
    public TMP_Text itemRarity;
    public TMP_Text itemValue;
    public TMP_Text itemDescription;
    public TMP_Text itemSlotText;

    [Header("Stats")]
    public GameObject itemStatObject;
    public GameObject itemStatsHolder;   

    private List<GameObject> activatedStats = new List<GameObject>();

    public void ShowItem(BaseItem item)
    {
        foreach (var statDisplay in activatedStats)
        {
            Destroy(statDisplay.gameObject);
        }

        if (item as BaseEquippable)
        {
            BaseEquippable equip = item as BaseEquippable;            

            foreach (var stat in equip.itemMods)
            {
                GameObject statObj = Instantiate(itemStatObject, itemStatsHolder.transform);
                TMP_Text statText = statObj.GetComponent<TMP_Text>();

                TextInfo info = new CultureInfo("en-US", false).TextInfo;

                if(stat.Value > 0)
                {
                    if (stat.Type == Modifier.StatModType.Flat)
                    {
                        statText.text = "+" + Mathf.RoundToInt(Mathf.Abs(stat.Value)) + " " + info.ToTitleCase(stat.statDisplayStringName) + ".";
                    }
                    else if (stat.Type == Modifier.StatModType.PercentAdd)
                    {
                        statText.text = "+" + Mathf.RoundToInt(Mathf.Abs(stat.Value)) + "% " + info.ToTitleCase(stat.statDisplayStringName) + ".";
                    }
                    else if (stat.Type == Modifier.StatModType.PercentMult)
                    {
                        statText.text = "" + Mathf.RoundToInt(Mathf.Abs(stat.Value)) + "% increased " + info.ToTitleCase(stat.statDisplayStringName) + ".";
                    }
                }
                else
                {
                    if (stat.Type == Modifier.StatModType.Flat)
                    {
                        statText.text = "-" + Mathf.RoundToInt(Mathf.Abs(stat.Value)) + " " + info.ToTitleCase(stat.statDisplayStringName) + ".";
                    }
                    else if (stat.Type == Modifier.StatModType.PercentAdd)
                    {
                        statText.text = "-" + Mathf.RoundToInt(Mathf.Abs(stat.Value)) + "% " + info.ToTitleCase(stat.statDisplayStringName) + ".";
                    }
                    else if (stat.Type == Modifier.StatModType.PercentMult)
                    {
                        statText.text = "" + Mathf.RoundToInt(Mathf.Abs(stat.Value)) + "% decreased " + info.ToTitleCase(stat.statDisplayStringName) + ".";
                    }
                }                

                activatedStats.Add(statObj);
            }

            itemName.text = item.itemName;
            itemStatsBar.enabled = true;
        }
        else
        {
            itemName.text = item.itemName + "(" + item.itemStack + ")";
            itemStatsBar.enabled = false;           
        }

        itemSlotText.text = "[" +item.itemType.ToString().Replace("_", " ") + "]";

        itemStatsBar.color = item.itemRarity.rarityColor;
        itemDescriptionBorder.color = item.itemRarity.rarityColor;
        itemIcon.sprite = item.itemIcon;
        SetBorder(item);
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
