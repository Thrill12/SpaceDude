using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Items/Weapons/Burst Gun")]
public class BurstShootingGun : StraightShootingGun
{
    public Stat bulletsInBurst;
    public Stat timeBetweenShots;

    public override void Attack(GameObject weaponObject,  AudioSource audioSource, WeaponsHolder holder, PlayerInput playerInput = null)
    {
        UIManager.instance.StartCoroutine(AttackCoroutine(weaponObject, playerInput, audioSource, holder));
    }

    IEnumerator AttackCoroutine(GameObject weaponObject, PlayerInput playerInput, AudioSource audioSource, WeaponsHolder holder)
    {
        for (int i = 0; i < bulletsInBurst.Value; i++)
        {
            if (currentBullets == 0) yield return null;
            base.Attack(weaponObject, audioSource, holder, playerInput);
            yield return new WaitForSeconds(timeBetweenShots.Value);
        }
    }
}
