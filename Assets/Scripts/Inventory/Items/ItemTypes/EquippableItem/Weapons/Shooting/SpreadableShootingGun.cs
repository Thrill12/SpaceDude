using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Items/Weapons/Spreadable Gun")]
public class SpreadableShootingGun : StraightShootingGun
{
    public Stat numberOfPelletsToShoot;
    public Stat spreadAngleFromCenterBarrel;

    public override void Attack(GameObject weaponObject, AudioSource audioSource, WeaponsHolder holder, PlayerInput playerInput)
    {
        if (currentBullets > 0)
        {
            audioSource.PlayOneShot(attackSound);
        }
        else
        {
            hostEntity.GetComponent<WeaponsHolder>().audioSource.PlayOneShot(outOfAmmoSound);
        }
        GameObject shootSource = weaponObject.transform.Find("AttackSource").gameObject;
        for (int i = 0; i < numberOfPelletsToShoot.Value; i++)
        {
            if (currentBullets == 0) return;

            GameObject pellet = Instantiate(projectile, shootSource.transform.position, shootSource.transform.rotation);

            float angle = Random.Range(-spreadAngleFromCenterBarrel.Value, spreadAngleFromCenterBarrel.Value);
            pellet.transform.Rotate(new Vector3(0, 0, angle));

            pellet.GetComponent<Rigidbody2D>().AddForce((projectileSpeed.Value + hostEntity.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude) * pellet.transform.up);

            pellet.GetComponent<Projectile>().entityShotFrom = hostEntity;
            pellet.GetComponent<Projectile>().weaponShotFrom = this;
            currentBullets -= 1;
            holder.ShakeCamera();
            Destroy(pellet, 3);
        }
        
    }
}
