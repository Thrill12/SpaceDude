using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName="Items/Weapons/Melee")]
public class BaseMelee : BaseWeapon
{
    
    public bool canReceiveInput = true;
    
    public bool receivedInput;

    public override void Attack(GameObject weaponObject, AudioSource audioSource, WeaponsHolder holder, PlayerInput playerInput = null)
    {
        weaponObject.GetComponent<TriggerEnterSwordGiveDamage>().hostEntity = hostEntity;
        weaponObject.GetComponent<TriggerEnterSwordGiveDamage>().weapon = this;        

        if (canReceiveInput)
        {
            receivedInput = true;
            canReceiveInput = false;
        }
        else
        {
            return;
        }
    }

    public void SwapInputs(GameObject weapon)
    {
        if (!canReceiveInput)
        {
            canReceiveInput = true;
        }
        else
        {
            canReceiveInput = false;
        }
    }

    public void TriggerTrue(GameObject weapon)
    {
        weapon.GetComponent<TriggerEnterSwordGiveDamage>().canDamage = true;
        hostEntity.GetComponent<AudioSource>().PlayOneShot(attackSound);
    }

    public void TriggerFalse(GameObject weapon)
    {
        weapon.GetComponent<TriggerEnterSwordGiveDamage>().canDamage = false;        
    }
}
