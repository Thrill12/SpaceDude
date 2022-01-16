using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Items/Weapons/Projectile Gun")]
public class StraightShootingGun : BaseWeapon
{
    public GameObject projectile;
    public Stat projectileSpeed;

    //A normal shoot function for a straight shooting gun
    public override void Attack(GameObject weaponObject, PlayerInput playerInput)
    {
        Vector3 lookDir = Vector3.zero;
        if(playerInput.currentActionMap.name == "KeyBoard")
        {
            lookDir = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        }
        else
        {
            lookDir = weaponObject.transform.up;
        }
        
        GameObject shootSource = weaponObject.transform.Find("AttackSource").gameObject;

        GameObject proj = Instantiate(projectile, shootSource.transform.position, Quaternion.identity);

        //Have to make it relative ofc
        proj.GetComponent<Rigidbody2D>().AddForce((projectileSpeed.Value + hostEntity.gameObject.GetComponent<Rigidbody2D>().velocity.magnitude) * lookDir.normalized);
        proj.GetComponent<Projectile>().entityShotFrom = hostEntity;
        proj.GetComponent<Projectile>().weaponShotFrom = this;

        Destroy(proj, 3);
    }
}
