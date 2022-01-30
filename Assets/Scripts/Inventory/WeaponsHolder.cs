using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;
using System.Linq;

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

    [Space(10)]

    public GameObject ammoDisplay;
    public TMP_Text currentBullets;
    public TMP_Text totalBulletsInInventory;

    private float nextFire;
    public AudioSource audioSource;
    private bool firing = false;
    private int totalBulletsAvailable;

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

        if(currentlyEquippedWeapon != null)
        {
            if (ammoDisplay.activeInHierarchy)
            {
                BaseGun gun = (BaseGun)currentlyEquippedWeapon;
                totalBulletsAvailable = inventory.playerInventoryItems.Where(x => x.itemType == gun.ammoType).Sum(x => x.itemStack);


                currentBullets.text = gun.currentBullets.ToString();
                totalBulletsInInventory.text = totalBulletsAvailable.ToString();
            }
        }
        else
        {
            ammoDisplay.SetActive(false);
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
    }

    public void Reload(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if(currentlyEquippedWeapon as BaseGun)
        {
            BaseGun gun = currentlyEquippedWeapon as BaseGun;
            if(gun.currentBullets < gun.maxBulletsInClip.Value)
            {
                if (totalBulletsAvailable > gun.maxBulletsInClip.Value)
                {
                    inventory.playerInventoryItems.Where(x => x.itemType == gun.ammoType).First().itemStack -= (int)gun.maxBulletsInClip.Value - gun.currentBullets;
                    gun.currentBullets = (int)gun.maxBulletsInClip.Value;
                }
                else
                {
                    int sum = inventory.playerInventoryItems.Where(x => x.itemType == gun.ammoType).Sum(x => x.itemStack);

                    if(sum > 0)
                    {
                        inventory.playerInventoryItems.Where(x => x.itemType == gun.ammoType).First().itemStack -= (int)gun.maxBulletsInClip.Value - gun.currentBullets;
                        inventory.playerInventoryItems.RemoveAll(x => x.itemType == gun.ammoType);
                        gun.currentBullets = sum;
                    }                    
                }
            }        
        }
    }

    //Void for calling shoot from input system events
    private void AttackVoid()
    {
        if(nextFire <= 0)
        {
            if (currentlyEquippedWeapon == null) return;

            currentlyEquippedWeapon.Attack(weaponObject, playerInput, audioSource, this);
            nextFire = currentlyEquippedWeapon.attackCooldown.Value;
        }       
    }

    //This swaps weapons when both are equipped, but is also called when the player picks up a weapon, so that they can immediately use the new one
    public void SwapWeapons(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SwapWeapons();
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

        if(currentlyEquippedWeapon as BaseGun)
        {
            ammoDisplay.SetActive(true);
        }
        else
        {
            ammoDisplay.SetActive(false);
        }

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
