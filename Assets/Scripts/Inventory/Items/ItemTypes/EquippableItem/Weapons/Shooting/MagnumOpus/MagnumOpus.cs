using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName ="Items/Weapons/Unique/Magnum Opus")]
public class MagnumOpus : StraightShootingGun
{
    public Stat ballRadius;
    public Stat tendrilCount;
    public Stat tendrilSpawnCooldown;

    public override void Attack(GameObject weaponObject, AudioSource audioSource, WeaponsHolder holder, PlayerInput playerInput = null)
    {
        Debug.Log("Attacking");
        //base.Attack(weaponObject, audioSource, holder, playerInput, hitEffects);

        if (ignoreAmmo == false && currentBullets == 0)
        {
            return;
        }

        Vector3 lookDir = Vector3.zero;
        lookDir = weaponObject.transform.up;

        GameObject shootSource = weaponObject.transform.Find("AttackSource").gameObject;

        GameObject proj = Instantiate(projectile, shootSource.transform.position, Quaternion.identity);

        //Have to make it relative ofc
        proj.GetComponent<Rigidbody2D>().velocity = hostEntity.gameObject.GetComponent<Rigidbody2D>().velocity;
        proj.GetComponent<Rigidbody2D>().AddForce((projectileSpeed.Value) * lookDir.normalized);
        proj.GetComponent<Projectile>().entityShotFrom = hostEntity;
        proj.GetComponent<Projectile>().weaponShotFrom = this;
        proj.GetComponent<Projectile>().damage = hostEntity.damageMultiplier.Value * damage.Value;
        proj.GetComponent<Projectile>().hitEffects = hitEffects;

        proj.GetComponent<LightningBoltSpawner>().radius = ballRadius.Value;
        proj.GetComponent<LightningBoltSpawner>().enemiesHitAtOnce = (int)tendrilCount.Value;
        proj.GetComponent<LightningBoltSpawner>().lightningSpawnCooldown = tendrilSpawnCooldown.Value;

        audioSource.PlayOneShot(attackSound);
        holder.ShakeCamera();
        currentBullets -= 1;
        Destroy(proj, proj.GetComponent<Projectile>().timeOut);
    }

    public override void OnLevelUp()
    {
        base.OnLevelUp();

        tendrilCount.RemoveAllModifiersFromSource(this);
        tendrilCount.AddModifier(new Modifier("DamageLevel", tendrilCount, tendrilCount.Value * Mathf.Pow(1.05f, itemLevel - 1) - tendrilCount.Value, Modifier.StatModType.Flat));
    }
}
