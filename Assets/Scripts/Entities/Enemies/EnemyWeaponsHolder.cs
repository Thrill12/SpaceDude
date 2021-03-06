using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseEnemy))]
public class EnemyWeaponsHolder : WeaponsHolder
{
    private Transform playerPosition;

    public override void Start()
    {
        try
        {
            playerPosition = GameObject.FindGameObjectWithTag("PlayerSuit").transform;
        }
        catch
        {
            Debug.Log("Couldnt find player transform");
        }
        
        audioSource = GetComponent<AudioSource>();
        try
        {
            GameManager.instance.LoadResourcesForItem(mainWeapon);
        }
        catch
        {
            Debug.Log("Game manager not in scene");
        }
        currentlyEquippedWeapon = Instantiate(mainWeapon);
        SwapWeapons();
        Debug.Log("TURRET SHOULD NOW HAVE EQUIPPED " + mainWeapon.name);
        Debug.Log("TURRET NOW HAS EQUIPPED " + currentlyEquippedWeapon.name);
    }

    public override void Update()
    {
        if(playerPosition == null && GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>().playerMovement.isPlayerPiloting == false)
        {
            try
            {
                playerPosition = GameObject.FindGameObjectWithTag("PlayerSuit").transform;
            }
            catch
            {

            }          
        }
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

        weaponObject = Instantiate(currentlyEquippedWeapon.weaponObject, weaponObjectPosition.transform.position, Quaternion.identity);
        weaponObject.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
        weaponObject.transform.rotation = gameObject.transform.rotation;
        currentlyEquippedWeapon.instantiatedWeapon = weaponObject;
        weaponObject.transform.parent = gameObject.transform;
    }
}
