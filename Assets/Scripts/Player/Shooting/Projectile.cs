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
        Destroy(gameObject, 3);
    }

    private void Update()
    {
        Vector2 dir = transform.GetComponent<Rigidbody2D>().velocity;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle + 90, Vector3.forward);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<BaseEntity>() && collision.GetComponent<BaseEntity>() != entityShotFrom || ((layerMask.value & (1 << collision.gameObject.layer)) > 0))
        {
            if (collision.gameObject.GetComponent<BaseEntity>() && collision.gameObject != entityShotFrom.gameObject)
            {
                collision.gameObject.GetComponent<BaseEntity>().TakeDamage(damage, collision.ClosestPoint(gameObject.transform.position), entityShotFrom);
            }

            if (collision.gameObject.GetComponent<Rigidbody2D>())
            {
                collision.gameObject.GetComponent<Rigidbody2D>().AddForce(GetComponent<Rigidbody2D>().velocity.normalized, ForceMode2D.Impulse);
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
