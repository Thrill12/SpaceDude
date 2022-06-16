using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float timeOut = 3;
    public BaseEntity entityShotFrom;
    public BaseWeapon weaponShotFrom;

    public GameObject impactParticles;

    public float damage;

    public LayerMask layerMask;

    [HideInInspector]
    public EffectToCheck hitEffects;

    public virtual void Start()
    {
        Destroy(gameObject, timeOut);
    }

    private void Update()
    {
        Vector2 dir = transform.GetComponent<Rigidbody2D>().velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.GetComponent<BaseEntity>() && collision.gameObject.GetComponent<BaseEntity>() != entityShotFrom || ((layerMask.value & (1 << collision.gameObject.layer)) > 0))
        {
            if (collision.gameObject.GetComponent<BaseEntity>() && collision.gameObject != entityShotFrom.gameObject)
            {
                collision.gameObject.GetComponent<BaseEntity>().TakeDamage(damage, gameObject.transform.position, entityShotFrom, false, hitEffects);
            }

            if (collision.gameObject.GetComponent<Rigidbody2D>())
            {
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().velocity.normalized, ForceMode2D.Impulse);
            }

            if (collision.gameObject != entityShotFrom.gameObject)
            {
                Explode();
            }
        }
    }

    public void Explode()
    {
        Instantiate(impactParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
