using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseGun : BaseWeapon
{
    public Stat maxBulletsInClip;
    public int currentBullets;

    public AudioClip outOfAmmoSound;
    [SerializeField, HideInInspector]
    public string outOfAmmoSoundFilePath;

    public GameObject projectile;
    [SerializeField, HideInInspector]
    public string projectilePath;

    public ItemType ammoType;
    public bool ignoreAmmo = false;

    public override void Attack(GameObject weaponObject, AudioSource audioSource, WeaponsHolder holder, PlayerInput playerInput = null)
    {
        if (currentBullets > 0)
        {
            
        }
        else
        {
            holder.audioSource.PlayOneShot(outOfAmmoSound);
        }
    }
}
