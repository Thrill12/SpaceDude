using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

public class BaseArmour : BaseEquippable
{
    public Stat armour;

    public override void OnEquip(BaseEntity hostEntity)
    {
        base.OnEquip(hostEntity);

        this.hostEntity = hostEntity;

        AddBonus();
    }

    public override void OnUnequip()
    {
        base.OnUnequip();

        RemoveBonus();

        isEquipped = false;
        hostEntity = null;
    }

    public void AddBonus()
    {
        Modifier armourMod = new Modifier("Armour", hostEntity.armour, armour.Value, Modifier.StatModType.Flat);
        armourMod.Source = this;

        hostEntity.armour.AddModifier(armourMod);

        Debug.Log("Bonus added");
    }

    public void RemoveBonus()
    {
        hostEntity.armour.RemoveAllModifiersFromSource(this);
    }

    public override void GenerateMods()
    {
        base.GenerateMods();       
    }

    public override void OnLevelUp()
    {
        base.OnLevelUp();

        RemoveBonus();

        armour.RemoveAllModifiersFromSource(this);
        armour.AddModifier(new Modifier("DamageLevel", armour, armour.Value * Mathf.Pow(1.25f, itemLevel - 1) - armour.Value, Modifier.StatModType.Flat));

        AddBonus();
    }

    //public override void AddMod(Modifier mod)
    //{
    //    base.AddMod(mod);
    //    itemMods.Add(mod);
    //}    

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
