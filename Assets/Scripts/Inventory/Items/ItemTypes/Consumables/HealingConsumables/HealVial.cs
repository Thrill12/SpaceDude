using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/Consumables/Healing Vial")]
public class HealVial : BaseConsumable
{
    public float healthToGive = 50;

    public override void Use()
    {
        if(GameObject.FindGameObjectWithTag("PlayerSuit").GetComponent<PlayerEntity>().health < GameObject.FindGameObjectWithTag("PlayerSuit").GetComponent<PlayerEntity>().maxHealth.Value)
        {
            base.Use();

            GameObject.FindGameObjectWithTag("PlayerSuit").GetComponent<PlayerEntity>().health += healthToGive;
        }
    }
}
