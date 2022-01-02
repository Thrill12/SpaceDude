using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class WeaponsHolder : MonoBehaviour
{
    public BaseWeapon mainWeapon;
    public BaseWeapon secondaryWeapon;

    private Inventory inventory;

    public BaseWeapon currentlyEquippedWeapon;
    public Transform weaponObjectPosition;
    public GameObject weaponAttackSource;
    public GameObject weaponObject;

    private float nextFire;
    private AudioSource audioSource;

    private void Start()
    {
        SwapWeapons();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            SwapWeapons();
        }

        if (nextFire <= 0)
        {
            if (Input.GetMouseButton(0))
            {
                Attack();
            }
        }

        nextFire -= Time.deltaTime;
    }

    //Uses the abstract function in the weapon to attack. Passed in the weapon object to allow the gun to have
    // access to its attack source, and any other damage or stats it needs
    public void Attack()
    {
        if (currentlyEquippedWeapon == null) return;

        Debug.Log(weaponObject.name);

        audioSource.PlayOneShot(currentlyEquippedWeapon.attackSound);
        currentlyEquippedWeapon.Attack(weaponObject);
        nextFire = currentlyEquippedWeapon.attackCooldown.Value;
    }

    //This swaps weapons when both are equipped, but is also called when the player picks up a weapon, so that they can immediately use the new one
    public void SwapWeapons()
    {
        Destroy(weaponObject);

        if (currentlyEquippedWeapon == mainWeapon)
        {
            currentlyEquippedWeapon = secondaryWeapon;
        }
        else if (currentlyEquippedWeapon == secondaryWeapon)
        {
            currentlyEquippedWeapon = mainWeapon;
        }
        else
        {
            currentlyEquippedWeapon = mainWeapon;
        }

        if (currentlyEquippedWeapon == null) return;

        weaponObject = Instantiate(currentlyEquippedWeapon.weaponObject, weaponObjectPosition.transform.position, transform.rotation);
        weaponObject.transform.parent = transform;

        nextFire = currentlyEquippedWeapon.attackCooldown.Value;

        Debug.Log("Current gun is " + currentlyEquippedWeapon.itemName);
        weaponAttackSource = GameObject.FindGameObjectWithTag("PlayerAttackSource");
    }

    public void StowWeaponWhenUnequipping(BaseItem weaponToStow)
    {
        if (weaponToStow == currentlyEquippedWeapon)
        {
            Destroy(weaponObject);
            currentlyEquippedWeapon = null;
        }
    }

    public void EquipWeapon(BaseWeapon weapon)
    {
        if (weapon.large)
        {
            if (mainWeapon == null)
            {
                mainWeapon = weapon;
            }
            else if (mainWeapon != null)
            {
                inventory.UnequipItem(mainWeapon);
                mainWeapon = weapon;
            }
        }
        else
        {
            if (secondaryWeapon == null)
            {
                secondaryWeapon = weapon;
            }
            else
            {
                inventory.UnequipItem(secondaryWeapon);
                secondaryWeapon = weapon;
            }
        }
    }

    public void UnequipWeapon(BaseWeapon weapon)
    {
        if (mainWeapon == weapon)
        {
            mainWeapon = null;
        }
        else if (secondaryWeapon == weapon)
        {
            secondaryWeapon = null;
        }
    }
}
