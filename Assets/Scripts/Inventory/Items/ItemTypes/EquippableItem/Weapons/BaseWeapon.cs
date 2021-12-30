using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseWeapon : BaseEquippable
{
    public GameObject weaponObject;
    public Stat damage;
    public Stat criticalChance;
    public Stat criticalDamage;
    public Stat attackCooldown;
    public bool large;

    public BaseWeapon()
    {
        itemType = ItemType.Weapon;
    }

    public abstract void Attack(GameObject weaponObject);
}
