using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Turret : BaseEnemy
{
    private bool isEnemyInRange = false;
    public float detectionRange;

    public List<string> enemyTags = new List<string>();

    private Light2D lighttt;

    public float nextFire = 0;
    public EnemyWeaponsHolder holder;

    private GameObject target;

    public override void Start()
    {
        base.Start();
        lighttt = GetComponentInChildren<Light2D>();
        holder = GetComponent<EnemyWeaponsHolder>();

        if (holder.currentlyEquippedWeapon as BaseGun)
        {
            BaseGun gun = holder.currentlyEquippedWeapon as BaseGun;
            gun.ignoreAmmo = true;
        }
    }

    public override void Update()
    {
        base.Update();

        RaycastHit2D hit;       

        if (target != null && isEnemyInRange)
        {
            hit = Physics2D.Raycast(transform.position, target.transform.position - transform.position);

            Debug.Log(hit.transform.tag);

            if (enemyTags.Contains(hit.transform.tag))
            {
                holder.RotateWeaponObject();
                if (nextFire <= 0)
                {
                    holder.AttackVoid();
                    Debug.Log("Turret has " + holder.currentlyEquippedWeapon.itemName + " equipped");
                    nextFire = holder.currentlyEquippedWeapon.attackCooldown.Value;
                }
                nextFire -= Time.deltaTime;
            }            
        }

        List<GameObject> nearbyEnemies = Physics2D.OverlapCircleAll(transform.position, detectionRange).ToList()
            .Where(x => enemyTags.Contains(x.transform.tag)).Select(x => x.gameObject).ToList(); 

        if (nearbyEnemies.Count > 0)
        {
            target = nearbyEnemies[Random.Range(0, nearbyEnemies.Count)];
            Debug.Log("Target found: " + target.gameObject.name);
            isEnemyInRange = true;
            lighttt.enabled = true;
        }
        else
        {
            target = null;
            isEnemyInRange = false;
            lighttt.enabled = false;
        }
    }
}
