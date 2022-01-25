using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(AudioSource))]
public class WeaponsHolder : MonoBehaviour
{
    public static WeaponsHolder instance;

    public BaseWeapon mainWeapon;
    public BaseWeapon secondaryWeapon;

    public PlayerInput playerInput;

    private Inventory inventory;

    public BaseWeapon currentlyEquippedWeapon;
    public Transform weaponObjectPosition;
    public GameObject weaponAttackSource;
    public GameObject weaponObject;

    private float nextFire;
    private AudioSource audioSource;
    private bool firing = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        inventory = Inventory.instance;
        SwapWeapons();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        nextFire -= Time.deltaTime;

        if(firing)
        {
            AttackVoid();
        }
    }

    //Uses the abstract function in the weapon to attack. Passed in the weapon object to allow the gun to have
    // access to its attack source, and any other damage or stats it needs
    public void Attack(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Performed || context.phase == InputActionPhase.Started)
        {
            firing = true;
        }
        else
        {
            firing = false;
        }
        Debug.Log(firing);
    }

    //Void for calling shoot from input system events
    private void AttackVoid()
    {
        if(nextFire <= 0)
        {
            if (currentlyEquippedWeapon == null) return;

            audioSource.PlayOneShot(currentlyEquippedWeapon.attackSound);
            currentlyEquippedWeapon.Attack(weaponObject, playerInput);
            nextFire = currentlyEquippedWeapon.attackCooldown.Value;
            ShakeCamera();
        }       
    }

    //This swaps weapons when both are equipped, but is also called when the player picks up a weapon, so that they can immediately use the new one
    public void SwapWeapons(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
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


            weaponAttackSource = GameObject.FindGameObjectWithTag("PlayerAttackSource");
        }      
    }

    //Swaps weapon used between primary and secondary
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


        weaponAttackSource = GameObject.FindGameObjectWithTag("PlayerAttackSource");
    }

    //Turns off the currently selected weapon
    public void StowWeaponWhenUnequipping(BaseItem weaponToStow)
    {
        if (weaponToStow == currentlyEquippedWeapon)
        {
            Destroy(weaponObject);
            currentlyEquippedWeapon = null;
        }
    }

    //Equips weapon in the argument in its appropiate slot, and automatically
    //unequips the item already in the slot, if there
    public void EquipWeapon(BaseWeapon weapon)
    {
        Debug.Log(weapon.itemName);

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

    //Unequips weapon selected
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

    public void ShakeCamera()
    {
        CinemachineImpulseSource impSource = GetComponent<CinemachineImpulseSource>();

        impSource.GenerateImpulse();
    }
}
