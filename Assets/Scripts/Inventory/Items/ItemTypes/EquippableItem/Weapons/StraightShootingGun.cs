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
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        GameObject shootSource = weaponObject.transform.Find("AttackSource").gameObject;
        Debug.Log(shootSource.name);
        GameObject proj = Instantiate(projectile, shootSource.transform.position, Quaternion.identity);
        Debug.Log(proj.name);
        //Have to make it relative ofc
        proj.GetComponent<Rigidbody2D>().AddForce((projectileSpeed.Value + hostEntity.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude) * -(weaponObject.transform.position - mousePos));
        proj.GetComponent<Projectile>().entityShotFrom = hostEntity;
        proj.GetComponent<Projectile>().weaponShotFrom = this;
        Debug.Log(hostEntity.gameObject.name);
        Destroy(proj, 3);
    }
}
