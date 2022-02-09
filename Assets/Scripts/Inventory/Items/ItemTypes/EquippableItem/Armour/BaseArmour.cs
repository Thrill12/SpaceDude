using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

[CreateAssetMenu(menuName ="Items/Armour")]
public class BaseArmour : BaseEquippable
{
    public override void OnEquip(BaseEntity hostEntity)
    {
        base.OnEquip(hostEntity);

        this.hostEntity = hostEntity;
        if (!isEquipped)
        {
            isEquipped = true;
        }

        foreach (var item in itemMods)
        {
            item.statAffecting = (Stat)typeof(BaseEntity).GetField(item.statStringName).GetValue(hostEntity);
            item.statAffecting.AddModifier(item);
        }
    }

    public override void OnUnequip()
    {
        isEquipped = false;
        hostEntity = null;

        //List<string> itemStatsStrings = hostEntity.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
        //        .Where(x => x.FieldType == typeof(Stat)).Select(x => x.Name).ToList();

        //List<Stat> allItemStats = new List<Stat>();

        //foreach (var str in itemStatsStrings)
        //{
        //    allItemStats.Add(GetFieldValue<Stat>(hostEntity, str));
        //}

        //allItemStats.Reverse();

        //foreach (var item in allItemStats)
        //{
        //    item.RemoveAllModifiersFromSource(this);
        //}

        foreach (var item in itemMods)
        {
            item.statAffecting.RemoveAllModifiersFromSource(this);
        }
    }

    public override void GenerateMods()
    {
        base.GenerateMods();

        for (int i = 0; i < UnityEngine.Random.Range(1, itemRarity.modAmount + 1); i++)
        {
            int randomProperty = UnityEngine.Random.Range(0, 5);
            switch (randomProperty)
            {
                case 0:

                    Modifier mod = new Modifier("Max Health", hostEntity.maxHealth, UnityEngine.Random.Range(5,100), Modifier.StatModType.Flat);
                    mod.Source = this;
                    mod.statStringName = "maxHealth";
                    AddMod(mod);

                    break;

                case 1:

                    mod = new Modifier("Armour", hostEntity.armour, UnityEngine.Random.Range(5, 100), Modifier.StatModType.PercentAdd);
                    mod.Source = this;
                    mod.statStringName = "armour";
                    AddMod(mod);

                    break;

                case 2:

                    mod = new Modifier("Damage Multiplier", hostEntity.damageMultiplier, UnityEngine.Random.Range(1f, 2f), Modifier.StatModType.PercentMult);
                    mod.Source = this;
                    mod.statStringName = "damageMultiplier";
                    AddMod(mod);

                    break;
                case 3:

                    mod = new Modifier("Health Regen", hostEntity.healthRegeneration, UnityEngine.Random.Range(5, 50), Modifier.StatModType.Flat);
                    mod.Source = this;
                    mod.statStringName = "healthRegeneration";
                    AddMod(mod);

                    break;
                case 4:

                    mod = new Modifier("Max Energy", hostEntity.maxEnergy, UnityEngine.Random.Range(5, 50), Modifier.StatModType.Flat);
                    mod.Source = this;
                    mod.statStringName = "maxEnergy";
                    AddMod(mod);

                    break;
                case 5:

                    mod = new Modifier("Energy Regen", hostEntity.energyRegeneration, UnityEngine.Random.Range(5, 50), Modifier.StatModType.Flat);
                    mod.Source = this;
                    mod.statStringName = "energyRegeneration";
                    AddMod(mod);

                    break;
            }
        }
    }

    public override void AddMod(Modifier mod)
    {
        base.AddMod(mod);
        itemMods.Add(mod);
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
