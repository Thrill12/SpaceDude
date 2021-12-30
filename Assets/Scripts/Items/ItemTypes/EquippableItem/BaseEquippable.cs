using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Equippable Item"), System.Serializable]
public class BaseEquippable : BaseItem
{
    public string itemSlot;

    [SerializeField]
    public List<Modifier> itemMods;

    public int itemLevel = 1;
    public float itemCurrentXP = 0;
    public float itemXPToNextLevel = 1000;
    private float xpBacklog;

    public BaseEntity hostEntity;
    public bool isEquipped = false;

    //Has to add a modifier to each stat that the item has mods for.
    public void OnEquip(BaseEntity entity)
    {
        hostEntity = entity;
        if (!isEquipped)
        {
            isEquipped = true;
            foreach (Modifier mod in itemMods)
            {
                mod.Source = this;
                Stat s = (Stat)typeof(BaseEntity).GetField(mod.statToModifyName).GetValue(hostEntity);
                s.AddModifier(mod);
            }
        }
    }

    //Has to clean up its modifiers, removing its modifiers from any stats it affected
    public void OnUnequip()
    {
        isEquipped = false;
        foreach (Modifier mod in itemMods)
        {
            Stat s = (Stat)typeof(BaseEntity).GetField(mod.statToModifyName).GetValue(hostEntity);
            s.RemoveAllModifiersFromSource(this);
        }
        hostEntity = null;
    }

    public void AddXP(float xp)
    {
        //Backlog of xp in case we receive more xp than required for next item level
        xpBacklog += xp;

        //Manages situation when we get more than enough xp to level up at least once, will keep leveling up.
        //using the XP it received.
        while (itemCurrentXP + xpBacklog > itemXPToNextLevel)
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
        itemXPToNextLevel = Mathf.Floor(1000 * Mathf.Pow(1.1352225792f, itemLevel - 1));
    }
}
