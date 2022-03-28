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
    

    public Image itemIcon;
    public Image itemBorder;
    public Image itemStatsBar;
    public Image itemDescriptionBorder;
    public TMP_Text itemName;
    public TMP_Text itemRarity;
    public TMP_Text itemValue;
    public TMP_Text itemDescription;
    public TMP_Text itemSlotText;

    [Header("Equippables")]

    public GameObject equippableStatsPage;
    public Image equippableXPBar;
    public TMP_Text equippableXP;
    public TMP_Text equippableLevel;

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

            equippableXPBar.fillAmount = equip.itemCurrentXP / equip.itemXPToNextLevel;
            equippableXP.text = $"<color=#718093>{equip.itemCurrentXP}<color=white>/<color=#e84118>{equip.itemXPToNextLevel}";
            equippableLevel.text = "Lv. " + equip.itemLevel;

            List<string> itemStatsStrings = equip.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.FieldType == typeof(Stat)).Select(x => x.Name).ToList();


            List<Stat> allItemStats = new List<Stat>();

            foreach (var str in itemStatsStrings)
            {
                allItemStats.Add(GetFieldValue<Stat>(equip, str));
            }

            allItemStats.Reverse();

            if (allItemStats.Count > 0)
            {
                foreach (var stat in allItemStats)
                {
                    GameObject displayer = Instantiate(itemStatModifierObject, itemStatsHolder.transform);
                    TMP_Text statText = displayer.GetComponent<TMP_Text>();

                    TextInfo info = new CultureInfo("en-US", false).TextInfo;

                    statText.text = stat.Value + " " + stat.statName;

                    activatedStats.Add(displayer);
                }

                if (equip as BaseGun)
                {
                    BaseGun gun = equip as BaseGun;

                    CheckForAndDisplayHitEffect(gun, EffectToCheck.bleed, "Bleeds", "#d63031");
                    CheckForAndDisplayHitEffect(gun, EffectToCheck.poison, "Poisons", "#44bd32");
                    CheckForAndDisplayHitEffect(gun, EffectToCheck.shock, "Shocks", "#0097e6");

                    GameObject displayer = Instantiate(itemStatModifierObject, itemStatsHolder.transform);
                    TMP_Text statText = displayer.GetComponent<TMP_Text>();
                    TextInfo info = new CultureInfo("en-US", false).TextInfo;
                    statText.text = "\n(Uses " + gun.ammoType.ToString().Replace("_", " ") + " for ammo)";
                    statText.fontSize -= 2;
                    activatedStats.Add(displayer);
                }
            }

            itemName.text = item.itemName;

            if (equip.itemMods.Any())
            {
                itemModifiersHolder.SetActive(false);
                foreach (var mod in equip.itemMods)
                {
                    GameObject modObj = Instantiate(itemStatModifierObject, itemStatsHolder.transform);
                    TMP_Text modText = modObj.GetComponent<TMP_Text>();

                    TextInfo info = new CultureInfo("en-US", false).TextInfo;

                    if (mod.Value > 0)
                    {
                        if (mod.Type == Modifier.StatModType.Flat)
                        {
                            modText.text = "+" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + " " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                        }
                        else if (mod.Type == Modifier.StatModType.PercentAdd)
                        {
                            modText.text = "+" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + "% " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                        }
                        else if (mod.Type == Modifier.StatModType.PercentMult)
                        {
                            modText.text = "" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + "% increased " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                        }
                    }
                    else
                    {
                        if (mod.Type == Modifier.StatModType.Flat)
                        {
                            modText.text = "-" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + " " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                        }
                        else if (mod.Type == Modifier.StatModType.PercentAdd)
                        {
                            modText.text = "-" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + "% " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                        }
                        else if (mod.Type == Modifier.StatModType.PercentMult)
                        {
                            modText.text = "" + Mathf.RoundToInt(Mathf.Abs(mod.Value)) + "% decreased " + info.ToTitleCase(mod.statDisplayStringName) + ".";
                        }
                    }

                    activatedModifiers.Add(modObj);
                }
            }
            else
            {
                itemModifiersHolder.SetActive(false);
            }

            
        }
        else
        {
            itemName.text = item.itemName + "(" + item.itemStack + ")";
            equippableStatsPage.SetActive(false);
        }

        itemSlotText.text = "[" +item.itemType.ToString().Replace("_", " ") + "]";
        itemIcon.sprite = item.itemIcon;
        itemRarity.text = item.itemRarity.rarityName;
        itemRarity.color = item.itemRarity.rarityColor;
        itemName.color = item.itemRarity.rarityColor;
        itemValue.text = "C:" + item.itemValue;
        itemDescription.text = item.itemDescription;
    }

    private void CheckForAndDisplayHitEffect(BaseGun gun, EffectToCheck effectToCheckFor, string text, string hexColor)
    {
        if ((gun.hitEffects & effectToCheckFor) != 0)
        {
            GameObject effectDisplayer = Instantiate(itemStatModifierObject, itemStatsHolder.transform);
            TMP_Text effectText = effectDisplayer.GetComponent<TMP_Text>();
            effectText.text = $"<color={hexColor}>{text}";
            effectText.fontSize -= 2;
            activatedStats.Add(effectDisplayer);
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
