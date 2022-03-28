using DigitalRuby.LightningBolt;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LightningBoltSpawner : Projectile
{
    public float radius;
    public int enemiesHitAtOnce = 1;

    public GameObject lightningBolt;
    public float lightningSpawnCooldown;

    private float nextSpawnTime;

    public bool shouldSpawnRandomBoltsInCircle = true;

    private void Update()
    {
        nextSpawnTime -= Time.deltaTime;

        if(nextSpawnTime <= 0)
        {
            nextSpawnTime = lightningSpawnCooldown;
            SpawnLightning(lightningSpawnCooldown);

            if (shouldSpawnRandomBoltsInCircle)
            {
                SpawnLightningInCircle();
            }
        }
    }

    public void SpawnLightning(float lightningBoltDestroyTimer)
    {
        List<GameObject> objs = Physics2D.OverlapCircleAll(transform.position, radius).Select(x => x.gameObject).ToList();

        int counter = enemiesHitAtOnce;

        foreach (var item in objs.Where(x => x.GetComponent<BaseEntity>() && x.GetComponent<BaseEntity>() != this.entityShotFrom))
        {
            if (counter <= 0) return;
            GameObject bolt = Instantiate(lightningBolt, transform.position, Quaternion.identity);
            bolt.GetComponent<LightningBoltScript>().StartObject = gameObject;

            bolt.GetComponent<LightningBoltScript>().EndObject = item;
            item.GetComponent<BaseEntity>().TakeDamage(damage, item.transform.position + (Vector3)Random.insideUnitCircle, entityShotFrom, false, hitEffects);
            Debug.Log("Timeout is " + lightningBoltDestroyTimer);
            gameObject.LeanScale(Vector3.zero, timeOut);

            Destroy(bolt, lightningBoltDestroyTimer);

            counter--;
        }
    }

    public void SpawnLightningInCircle()
    {
        for (int i = 0; i < 20; i++)
        {
            GameObject bolt = Instantiate(lightningBolt, transform);
            bolt.GetComponent<LightningBoltScript>().StartPosition = gameObject.transform.position;

            bolt.GetComponent<LightningBoltScript>().EndPosition = transform.position + (Vector3)Random.insideUnitCircle;
            Debug.Log("Spawned in circle");
        }
    }
}
