using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ExplosiveBarrel : BaseEnemy
{
    public float explosionRadius;
    public float explosionForce;
    public GameObject explosionEffect;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
    }

    public override void Die()
    {
        List<GameObject> nearby = Physics2D.OverlapCircleAll(transform.position, explosionRadius).Select(x => x.gameObject).ToList();

        foreach (var item in nearby)
        {
            if (item.GetComponent<Rigidbody2D>())
            {
                item.GetComponent<Rigidbody2D>().AddForce((item.transform.position - transform.position) * explosionForce, ForceMode2D.Impulse);
            }

            if (item.GetComponent<BaseEntity>())
            {
                item.GetComponent<BaseEntity>().TakeDamage(explosionForce * 50, (Vector2)item.transform.position + Random.insideUnitCircle, this);
            }
        }

        GameObject explosion = Instantiate(explosionEffect, transform.position, Quaternion.identity);
        explosion.GetComponent<AudioSource>().PlayOneShot(PrefabManager.instance.explosionSound);

        base.Die();
    }
}
