using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;
using TMPro;
using System.Linq;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class WeaponsHolder : MonoBehaviour
{
    public BaseWeapon mainWeapon;
    public BaseWeapon secondaryWeapon;

    public PlayerInput playerInput;

    public PlayerInventory inventory;

    public BaseWeapon currentlyEquippedWeapon;
    public Transform weaponObjectPosition;
    public GameObject weaponObject;

    [Space(10)]

    public GameObject ammoDisplay;
    public TMP_Text currentBullets;
    public TMP_Text totalBulletsInInventory;
    public Image weaponEquippedIcon;
    public Image cooldownIndicator;   

    private float nextFire;
    public AudioSource audioSource;
    private bool firing = false;
    private int totalMagazinesAvailable;

    private UIManager uiManager;

    public virtual void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Inventory").GetComponent<PlayerInventory>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        SwapWeapons();
        audioSource = GetComponent<AudioSource>();
    }

    public virtual void Update()
    {
        nextFire -= Time.deltaTime;

        if(firing)
        {
            AttackVoid();
        }

        if(currentlyEquippedWeapon != null && !uiManager.isInUI)
        {
            ammoDisplay.SetActive(true);
            if (ammoDisplay.activeInHierarchy)
            {
                weaponEquippedIcon.sprite = currentlyEquippedWeapon.itemIcon;
                cooldownIndicator.fillAmount = nextFire / currentlyEquippedWeapon.attackCooldown.Value;

                if(currentlyEquippedWeapon as BaseGun)
                {
                    BaseGun gun = (BaseGun)currentlyEquippedWeapon;
                    totalMagazinesAvailable = inventory.playerInventoryItems.Where(x => x.itemType == gun.ammoType).Sum(x => x.itemStack);

                    
                    currentBullets.text = gun.currentBullets.ToString();
                    totalBulletsInInventory.text = (totalMagazinesAvailable * (int)gun.maxBulletsInClip.Value).ToString();
                    
                }
                else
                {
                    currentBullets.text = "";
                    totalBulletsInInventory.text = "";
                }
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

        ReloadVoid();
    }

    public void ReloadVoid()
    {
        if (currentlyEquippedWeapon as BaseGun)
        {
            BaseGun gun = currentlyEquippedWeapon as BaseGun;
            if (gun.currentBullets < gun.maxBulletsInClip.Value)
            {
                if(inventory.playerInventoryItems.Where(x => x.itemType == gun.ammoType).Count() >= 1)
                {
                    inventory.playerInventoryItems.Where(x => x.itemType == gun.ammoType).Last().itemStack -= 1;
                    gun.currentBullets = (int)gun.maxBulletsInClip.Value;
                }               
            }
        }
    }

    //Void for calling shoot from input system events
    public virtual void AttackVoid()
    {        
        if(nextFire <= 0)
        {
            if (currentlyEquippedWeapon == null) return;

            if (currentlyEquippedWeapon as BaseGun)
            {
                BaseGun gun = (BaseGun)currentlyEquippedWeapon;
                if (gun.currentBullets == 0)
                {
                    if(inventory.playerInventoryItems.Where(x => x.itemType == gun.ammoType).Sum(x => x.itemStack) != 0)
                    {
                        ReloadVoid();                       
                    }
                    else
                    {
                        audioSource.PlayOneShot(gun.outOfAmmoSound);
                    }
                }
                else
                {
                    currentlyEquippedWeapon.Attack(weaponObject, audioSource, this, playerInput);
                    nextFire = currentlyEquippedWeapon.attackCooldown.Value;
                }
            }                   
        }       
    }

    //This swaps weapons when both are equipped, but is also called when the player picks up a weapon, so that they can immediately use the new one
    public void SwapWeapons(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            SwapWeapons();
            inventory.player.CheckItemsEquippedSprites();
        }      
    }

    //Swaps weapon used between primary and secondary
    public virtual void SwapWeapons()
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

        weaponObject = Instantiate(currentlyEquippedWeapon.weaponObject, weaponObjectPosition.transform.position, Quaternion.identity);
        weaponObject.transform.rotation = inventory.player.transform.rotation;
        currentlyEquippedWeapon.instantiatedWeapon = weaponObject;

        weaponObject.transform.parent = inventory.player.transform;

        nextFire = currentlyEquippedWeapon.attackCooldown.Value;

        if(ammoDisplay != null)
        {
            if (currentlyEquippedWeapon as BaseWeapon)
            {
                ammoDisplay.SetActive(true);
            }
            else
            {
                ammoDisplay.SetActive(false);
            }
        }      
    }

    //Turns off the currently selected weapon
    public void StowWeaponWhenUnequipping(BaseItem weaponToStow)
    {
        if (weaponToStow == currentlyEquippedWeapon)
        {
            currentlyEquippedWeapon.instantiatedWeapon = null;
            Destroy(weaponObject);
            currentlyEquippedWeapon = null;
            inventory.player.CheckItemsEquippedSprites();
        }
    }

    //Equips weapon in the argument in its appropiate slot, and automatically
    //unequips the item already in the slot, if there
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
        CinemachineImpulseSource impSource = GameObject.FindGameObjectWithTag("PlayerSuit").GetComponent<CinemachineImpulseSource>();

        impSource.GenerateImpulse();
    }
}
