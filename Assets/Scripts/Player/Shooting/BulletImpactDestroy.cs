using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletImpactDestroy : MonoBehaviour
{
    public GameObject impactParticles;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Wall"))
        {
            Explode();
        }
    }

    public void Explode()
    {
        Instantiate(impactParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
