using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public BaseEntity entityShotFrom;
    public BaseWeapon weaponShotFrom;

    public GameObject impactParticles;

    public float damage;

    public LayerMask layerMask;

    private void Start()
    {
        damage = weaponShotFrom.damage.Value * entityShotFrom.damageMultiplier.Value;
        Destroy(gameObject, 3);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BaseEntity>() && collision.GetComponent<BaseEntity>() != entityShotFrom || ((layerMask.value & (1 << collision.gameObject.layer)) > 0))
        {
            if (collision.gameObject.GetComponent<BaseEntity>())
            {
                collision.gameObject.GetComponent<BaseEntity>().TakeDamage(damage);
            }

            Explode();
            Destroy(gameObject);
        }
    }

    public void Explode()
    {
        Instantiate(impactParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
