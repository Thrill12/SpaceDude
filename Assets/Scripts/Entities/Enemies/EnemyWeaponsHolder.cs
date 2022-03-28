using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseEnemy))]
public class EnemyWeaponsHolder : WeaponsHolder
{
    private Transform playerPosition;

    public override void Start()
    {
        playerPosition = GameObject.FindGameObjectWithTag("PlayerSuit").transform;
        audioSource = GetComponent<AudioSource>();
        currentlyEquippedWeapon = ScriptableObject.Instantiate(mainWeapon);
        SwapWeapons();
        SwapWeapons();
    }

    public override void Update()
    {
        
    }

    public override void AttackVoid()
    {
        if (currentlyEquippedWeapon == null) return;
        
        currentlyEquippedWeapon.Attack(weaponObject, audioSource, this);
        Debug.Log("Enemy Weapons Holder");
    }

    public void RotateWeaponObject()
    {
        Vector3 objectPos = transform.position;
        Vector3 targetPos = playerPosition.position;

        Vector3 direction = new Vector3();

        direction.x = targetPos.x - objectPos.x;
        direction.y = targetPos.y - objectPos.y;

        //Calculate the angle the turret needs to be rotated to.
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        //Apply the calculated angle to the player.
        weaponObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    public override void SwapWeapons()
    {
        if(weaponObject != null)
        {
            Destroy(weaponObject);
        }

        currentlyEquippedWeapon = Instantiate(mainWeapon);
        currentlyEquippedWeapon.hostEntity = gameObject.GetComponent<BaseEntity>();
        if(currentlyEquippedWeapon as BaseGun)
        {
            BaseGun gun = currentlyEquippedWeapon as BaseGun;
            gun.ignoreAmmo = true;
        }

        weaponObject = Instantiate(currentlyEquippedWeapon.weaponObject, weaponObjectPosition.transform);
        weaponObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
    }
}
