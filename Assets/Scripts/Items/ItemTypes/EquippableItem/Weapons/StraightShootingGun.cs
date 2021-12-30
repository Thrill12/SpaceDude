using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Weapons/Projectile Gun")]
public class StraightShootingGun : BaseWeapon
{
    public GameObject projectile;
    public Stat projectileSpeed;

    public override void Attack(GameObject weaponObject)
    {
        GameObject shootSource = weaponObject.transform.Find("AttackSource").gameObject;
        GameObject proj = Instantiate(projectile, shootSource.transform.position, Quaternion.identity);
        //Have to make it relative ofc
        proj.GetComponent<Rigidbody2D>().AddForce((projectileSpeed.Value + hostEntity.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude) * shootSource.transform.up);
        Destroy(proj, 3);
    }
}
