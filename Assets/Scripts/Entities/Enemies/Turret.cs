using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Turret : BaseEnemy
{
    private bool isPlayerInRange = false;
    public float detectionRange;

    private Light2D lighttt;

    public float nextFire = 0;
    public EnemyWeaponsHolder holder;

    public override void Start()
    {
        base.Start();
        lighttt = GetComponentInChildren<Light2D>();
        holder = GetComponent<EnemyWeaponsHolder>();

        if (holder.currentlyEquippedWeapon as BaseGun)
        {
            BaseGun gun = holder.currentlyEquippedWeapon as BaseGun;
            gun.ignoreAmmo = true;
        }
    }

    public override void Update()
    {
        base.Update();                

        if (isPlayerInRange)
        {
            holder.RotateWeaponObject();
            if(nextFire <= 0)
            {
                holder.AttackVoid();
                Debug.Log("Turret has " + holder.currentlyEquippedWeapon.itemName + " equipped");
                nextFire = holder.currentlyEquippedWeapon.attackCooldown.Value;
            }
            nextFire -= Time.deltaTime;
        }

        if (Physics2D.OverlapCircleAll(transform.position, detectionRange).ToList().Any(x => x.gameObject.CompareTag("PlayerSuit")))
        {
            isPlayerInRange = true;
            lighttt.enabled = true;
        }
        else
        {
            isPlayerInRange = false;
            lighttt.enabled = false;
        }
    }
}
