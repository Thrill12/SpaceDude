using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerEnterSwordGiveDamage : MonoBehaviour
{
    public bool canDamage;
    public BaseEntity hostEntity;
    public BaseWeapon weapon;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (canDamage)
        {
            if(collision.GetComponent<BaseEntity>() && collision.GetComponent<BaseEntity>() != hostEntity)
            {
                BaseEntity entityHit = collision.GetComponent<BaseEntity>();
                entityHit.TakeDamage(weapon.damage.Value * hostEntity.damageMultiplier.Value, collision.ClosestPoint(gameObject.transform.position) + Random.insideUnitCircle / 2, hostEntity);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (canDamage)
        {
            if (collision.GetComponent<BaseEntity>() && collision.GetComponent<BaseEntity>() != hostEntity)
            {
                BaseEntity entityHit = collision.GetComponent<BaseEntity>();
                entityHit.TakeDamageNoSound(weapon.damage.Value / 15 * hostEntity.damageMultiplier.Value, collision.ClosestPoint(gameObject.transform.position) + Random.insideUnitCircle / 2, hostEntity);
            }
        }
    }
}
