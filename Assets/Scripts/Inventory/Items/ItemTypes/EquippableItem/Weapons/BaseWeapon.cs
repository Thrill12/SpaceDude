using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseWeapon : BaseEquippable
{
    public GameObject weaponObject;
    public Stat damage;
    public Stat criticalChance;
    public Stat criticalDamage;
    public Stat attackCooldown;
    public bool large;
    public AudioClip attackSound;

    public BaseWeapon()
    {
        itemType = ItemType.Weapon;
    }

    //Currently, weapons will only affect their own stats and will not affect the player - thats
    //the equipment's job

    //public void ActivateModifiers()
    //{
    //    foreach (Modifier mod in itemMods)
    //    {
    //        mod.Source = this;
    //        //Finding the stat for which its name is the same as the mod's stat it modifies
    //        Stat s = (Stat)typeof(BaseWeapon).GetField(mod.statToModifyName).GetValue(this);
    //        s.AddModifier(mod);
    //        Debug.Log("Added modifier to " + mod.statToModifyName + " of " + s.Value);
    //    }
    //}

    public override void OnEquip(BaseEntity entity)
    {
        hostEntity = entity;
        if (!isEquipped)
        {
            isEquipped = true;         
        }
    }    

    public override void OnUnequip()
    {
        isEquipped = false;
        hostEntity = null;
    }

    public override void AddMod(Modifier mod)
    {       
        itemMods.Add(mod);
    }

    //Abstract function that other weapons derived from here will need to use to attack
    public abstract void Attack(GameObject weaponObject, PlayerInput playerInput);
}
