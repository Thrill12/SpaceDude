using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEntity : MonoBehaviour
{
    public string entityName;

    public float health;
    public Stat maxHealth;
    public Stat armour;
    public Stat damageMultiplier;
    public Stat healthRegeneration;
    public Stat energy;
    public Stat energyRegeneration;

    float nextFireHealthRegen;
    float nextFireEnergyRegen;

    [HideInInspector]
    public PrefabManager pfMan;

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
        if (health <= 0)
        {
            Die();
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
        energy._value += energyToRegen;
        nextFireEnergyRegen = 1;
    }

    public virtual void TakeDamage(float damageToTake)
    {
        if(health > 0)
        {
            health -= damageToTake;           
        }        
    }

    public virtual void Die()
    {
        Destroy(gameObject);
    }
}
