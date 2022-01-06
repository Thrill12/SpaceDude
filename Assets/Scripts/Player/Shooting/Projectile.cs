using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public BaseEntity entityShotFrom;
    public BaseWeapon weaponShotFrom;

    public float damage;

    private void Start()
    {
        damage = weaponShotFrom.damage.Value * entityShotFrom.damageMultiplier.Value;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BaseEntity>() && collision.GetComponent<BaseEntity>() != entityShotFrom)
        {
            collision.gameObject.GetComponent<BaseEntity>().TakeDamage(damage);
            Debug.Log(collision.gameObject.name);
            Destroy(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
