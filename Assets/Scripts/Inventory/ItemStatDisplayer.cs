using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemStatDisplayer : MonoBehaviour
{
    public GameObject equippableStatsPage;
    public Image itemIcon;
    public Image itemBorder;
    public Image itemModifiersBar;
    public Image itemStatsBar;
    public Image itemDescriptionBorder;
    public TMP_Text itemName;
    public TMP_Text itemRarity;
    public TMP_Text itemValue;
    public TMP_Text itemDescription;
    public TMP_Text itemSlotText;

    [Header("Stats")]
    public GameObject itemStatModifierObject;
    public GameObject itemModifiersHolder;
    public GameObject itemStatsHolder;

    private List<GameObject> activatedModifiers = new List<GameObject>();
    private List<GameObject> activatedStats = new List<GameObject>();

    public void ShowItem(BaseItem item)
    {
        foreach (var modDisplay in activatedModifiers)
        {
            Destroy(modDisplay.gameObject);
        }

        foreach (var statDisplay in activatedStats)
        {
            Destroy(statDisplay.gameObject);
        }

        if (item as BaseEquippable)
        {
            BaseEquippable equip = item as BaseEquippable;
            equippableStatsPage.SetActive(true);
            SetBorder(equip, equippableStatsPage.GetComponent<Image>());

            foreach (var mod in equip.itemMods)
            {
                GameObject statObj = Instantiate(itemStatModifierObject, itemModifiersHolder.transform);
                TMP_Text statText = statObj.GetComponent<TMP_Text>();

                TextInfo info = new CultureInfo("en-US", false).TextInfo;

                if(mod.Value > 0)
                {
                    if (mod.Type == Modifier.StatModType.Flat)
                    {
                        statText.text = "+" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + " " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                    }
                    else if (mod.Type == Modifier.StatModType.PercentAdd)
                    {
                        statText.text = "+" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + "% " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                    }
                    else if (mod.Type == Modifier.StatModType.PercentMult)
                    {
                        statText.text = "" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + "% increased " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                    }
                }
                else
                {
                    if (mod.Type == Modifier.StatModType.Flat)
                    {
                        statText.text = "-" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + " " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                    }
                    else if (mod.Type == Modifier.StatModType.PercentAdd)
                    {
                        statText.text = "-" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + "% " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                    }
                    else if (mod.Type == Modifier.StatModType.PercentMult)
                    {
                        statText.text = "" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + "% decreased " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                    }
                }                

                activatedModifiers.Add(statObj);
            }

            List<string> itemStatsStrings = equip.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.FieldType == typeof(Stat)).Select(x => x.Name).ToList();

            List<Stat> allItemStats = new List<Stat>();

            foreach (var str in itemStatsStrings)
            {
                allItemStats.Add(GetFieldValue<Stat>(equip, str));
            }

            allItemStats.Reverse();

            foreach (var stat in allItemStats)
            {
                GameObject displayer = Instantiate(itemStatModifierObject, itemStatsHolder.transform);
                TMP_Text statText = displayer.GetComponent<TMP_Text>();

                TextInfo info = new CultureInfo("en-US", false).TextInfo;

                statText.text = stat.Value + " " + stat.statName;

                activatedStats.Add(displayer);
            }

            if (equip.GetType().GetField("ammoType") != null)
            {
                GameObject displayer = Instantiate(itemStatModifierObject, itemStatsHolder.transform);
                
                TMP_Text statText = displayer.GetComponent<TMP_Text>();

                TextInfo info = new CultureInfo("en-US", false).TextInfo;

                statText.text = "\n(Uses " + equip.GetType().GetField("ammoType").GetValue(equip).ToString().Replace("_", " ") + "s for ammo)";
                statText.fontSize -= 2;

                activatedStats.Add(displayer);
            }

            itemName.text = item.itemName;
            itemModifiersBar.enabled = true;
        }
        else
        {
            itemName.text = item.itemName + "(" + item.itemStack + ")";
            itemModifiersBar.enabled = false;
            equippableStatsPage.SetActive(false);
        }

        itemSlotText.text = "[" +item.itemType.ToString().Replace("_", " ") + "]";

        itemModifiersBar.color = item.itemRarity.rarityColor;
        itemDescriptionBorder.color = item.itemRarity.rarityColor;
        itemIcon.sprite = item.itemIcon;
        SetBorder(item, itemBorder);
        itemRarity.text = item.itemRarity.name;
        itemRarity.color = item.itemRarity.rarityColor;
        itemValue.text = "C:" + item.itemValue;
        itemDescription.text = item.itemDescription;
    }

    public void SetBorder(BaseItem item, Image itemBorder)
    {
        if(item.itemRarity.rarityName == "Common")
        {
            itemBorder.sprite = PrefabManager.instance.commonItemBorder;
        }
        else if (item.itemRarity.rarityName == "Rare")
        {
            itemBorder.sprite = PrefabManager.instance.rareItemBorder;
        }
        else if(item.itemRarity.rarityName == "Royal")
        {
            itemBorder.sprite = PrefabManager.instance.royalItemBorder;
        }
    }

    public static T GetFieldValue<T>(object obj, string fieldName)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");

        var field = obj.GetType().GetField(fieldName, BindingFlags.Public |
                                                      BindingFlags.NonPublic |
                                                      BindingFlags.Instance);

        if (field == null)
            throw new ArgumentException("fieldName", "No such field was found.");

        if (!typeof(T).IsAssignableFrom(field.FieldType))
            throw new InvalidOperationException("Field type and requested type are not compatible.");

        return (T)field.GetValue(obj);
    }
}
