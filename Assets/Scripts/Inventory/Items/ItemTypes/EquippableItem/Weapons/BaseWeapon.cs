using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class BaseWeapon : BaseEquippable
{
    public GameObject weaponObject;
    [SerializeField, HideInInspector]
    public string weaponObjectPath;

    public GameObject instantiatedWeapon;

    public AudioClip attackSound;
    [SerializeField, HideInInspector]
    public string attackSoundPath;

    [EnumMask]
    public EffectToCheck hitEffects;

    public Stat damage;
    public Stat criticalChance;
    public Stat criticalDamage;
    public Stat attackCooldown;
    public bool large;

    public BaseWeapon()
    {
        itemType = ItemType.Weapon;
    }

    public override void OnEquip(BaseEntity entity)
    {
        base.OnEquip(entity);

        if (itemMods.Count == 0) return;

        foreach (var item in itemMods)
        {
            item.statAffecting = (Stat)typeof(BaseWeapon).GetField(item.statStringName).GetValue(this);
            item.statAffecting.AddModifier(item);
        }
    }    

    public override void OnUnequip()
    {
        isEquipped = false;
        hostEntity = null;
    }

    public override void OnLevelUp()
    {
        base.OnLevelUp();

        damage.RemoveAllModifiersFromSource(this);
        damage.AddModifier(new Modifier("DamageLevel", damage, damage.Value * Mathf.Pow(1.05f, itemLevel - 1) - damage.Value, Modifier.StatModType.Flat));
    }

    //public override void AddMod(Modifier mod)
    //{       
    //    itemMods.Add(mod);
    //}

    //Abstract function that other weapons derived from here will need to use to attack
    public abstract void Attack(GameObject weaponObject, AudioSource audioSource, WeaponsHolder holder, PlayerInput playerInput = null);
}
