using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BaseGun : BaseWeapon
{
    public Stat maxBulletsInClip;
    public int currentBullets;
    public AudioClip outOfAmmoSound;
    public ItemType ammoType;

    public override void Attack(GameObject weaponObject, PlayerInput playerInput, AudioSource audioSource, WeaponsHolder holder)
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
