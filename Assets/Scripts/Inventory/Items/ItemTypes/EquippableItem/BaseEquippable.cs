using FullSerializer;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable Item"), Serializable]
public class BaseEquippable : BaseItem
{
    public const int MAXMODLIMIT = 4;

    [Tooltip("The name of the slot that the item will go into. Has to be the same as the name of the slot in the SO")]
    public string itemSlot;

    [SerializeField, Tooltip("Current mods for this specific item")]
    public List<Modifier> itemMods;

    public Stat xpMultiplier;

    public int itemLevel = 1;
    public float itemCurrentXP = 0;
    public float itemXPToNextLevel = 1000;
    private float xpBacklog;

    [fsIgnore]
    public BaseEntity hostEntity;
    public bool isEquipped;

    public virtual void OnEquip(BaseEntity hostEntity)
    {
        this.hostEntity = hostEntity;
        if (!isEquipped)
        {
            isEquipped = true;
        }        
    }

    public virtual void OnUnequip()
    {

    }

    public virtual void AddMod(Modifier mod)
    {
        itemMods.Add(mod);
    }

    public void RemoveMod(Modifier mod)
    {
        mod.statAffecting.RemoveModifier(mod);
    }

    public virtual void GenerateMods()
    {
        //Each subclass of weapon will have its own stats that need generating, so all the generation is handled in that, this is just for ease of calling.
    }

    public void AddXP(float xp)
    {
        Debug.Log("Adding " + xp);

        //Backlog of xp in case we receive more xp than required for next item level
        xpBacklog += xp * xpMultiplier.Value;

        //Manages situation when we get more than enough xp to level up at least once, will keep leveling up.
        //using the XP it received.
        while (itemCurrentXP + xpBacklog >= itemXPToNextLevel)
        {
            float differenceToNextLevel = itemXPToNextLevel - itemCurrentXP;
            xpBacklog -= differenceToNextLevel;
            LevelUp();
            itemCurrentXP = 0;
        }

        itemCurrentXP += xpBacklog;
        xpBacklog = 0;

        itemCurrentXP = Mathf.Floor(itemCurrentXP);
    }

    //That weird constant was chosen because it made req. XP for level 50 to be 500k, which was kinda perfect ;)
    public void LevelUp()
    {
        itemLevel++;
        itemXPToNextLevel = Mathf.Floor(1000 * Mathf.Pow(1.69897001f, itemLevel - 1));

        OnLevelUp();
    }

    public virtual void OnLevelUp()
    {
        itemValue *= itemLevel;
    }
}
