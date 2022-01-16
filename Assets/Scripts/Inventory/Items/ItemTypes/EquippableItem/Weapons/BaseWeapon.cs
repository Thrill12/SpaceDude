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

    //Abstract function that other weapons derived from here will need to use to attack
    public abstract void Attack(GameObject weaponObject, PlayerInput playerInput);
}
