using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleBehaviour : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (Inventory.instance.weaponHolder.currentlyEquippedWeapon as BaseMelee)
        {
            BaseMelee weapon = Inventory.instance.weaponHolder.currentlyEquippedWeapon as BaseMelee;

            weapon.canReceiveInput = true;
            weapon.receivedInput = false;
            weapon.TriggerFalse(weapon.instantiatedWeapon);
        }
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if(Inventory.instance.weaponHolder.currentlyEquippedWeapon as BaseMelee)
        {
            BaseMelee weapon = Inventory.instance.weaponHolder.currentlyEquippedWeapon as BaseMelee;

            if (weapon.receivedInput)
            {
                weapon.TriggerTrue(weapon.instantiatedWeapon);
                animator.SetTrigger("AttackOne");
                weapon.SwapInputs(animator.gameObject);
                weapon.receivedInput = false;
            }
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    if (Inventory.instance.weaponHolder.currentlyEquippedWeapon as BaseMelee)
    //    {
    //        BaseMelee weapon = Inventory.instance.weaponHolder.currentlyEquippedWeapon as BaseMelee;

    //        weapon.TriggerFalse(weapon.instantiatedWeapon);
    //    }
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
