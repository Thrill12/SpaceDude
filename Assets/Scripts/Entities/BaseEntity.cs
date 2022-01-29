using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BaseEntity : MonoBehaviour
{
    public int ID;
    public string entityName;

    public float health;
    public Stat maxHealth;
    public Stat armour;
    public Stat damageMultiplier;
    public Stat healthRegeneration;
    public float energy;
    public Stat maxEnergy;
    public Stat energyRegeneration;

    float nextFireHealthRegen;
    float nextFireEnergyRegen;

    [HideInInspector]
    public PrefabManager pfMan;

    [HideInInspector]
    public bool isDead;

    private void Awake()
    {
        health = maxHealth.Value;
    }

    public virtual void Start()
    {
        pfMan = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
    }

    public virtual void Update()
    {
        if (health <= 0 && !isDead)
        {
            Die();
            isDead = true;
        }

        if (nextFireHealthRegen <= 0)
        {
            RegenHealth(healthRegeneration.Value);
        }

        if (nextFireEnergyRegen <= 0)
        {
            RegenHealth(energyRegeneration.Value);
        }

        nextFireHealthRegen -= Time.deltaTime;
        nextFireEnergyRegen -= Time.deltaTime;
    }

    public virtual void RegenHealth(float healthToRegen)
    {
        health += healthToRegen;
        nextFireHealthRegen = 1;
    }

    public virtual void RegenEnergy(float energyToRegen)
    {
        maxEnergy._value += energyToRegen;
        nextFireEnergyRegen = 1;
    }

    public virtual void TakeDamage(float damageToTake)
    {
        if(health > 0)
        {
            health -= UnityEngine.Random.Range(damageToTake - 2, damageToTake + 2);
            PrefabManager.instance.SpawnNumberPopup(damageToTake, PrefabManager.instance.orange, (Vector2)transform.position + UnityEngine.Random.insideUnitCircle);
        }        
    }

    public virtual void Die()
    {
        GameEvents.instance.OnEntityKilled(this);
        Destroy(gameObject);
    }
}
