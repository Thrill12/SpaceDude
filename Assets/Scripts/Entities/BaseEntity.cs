using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(AudioSource))]
public class BaseEntity : MonoBehaviour
{
    public string ID;
    public string entityName;

    public float health;
    public Stat maxHealth;
    public Stat armour;
    public Stat damageMultiplier;
    public Stat healthRegeneration;
    public float energy;
    public Stat maxEnergy;
    public Stat energyRegeneration;

    [Space(5)]

    public AudioClip hitSound;

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

    public virtual void TakeDamage(float damageToTake, Vector3 popupPosition, BaseEntity hitter, bool ignoreEffect = false)
    {
        GetComponent<AudioSource>().PlayOneShot(hitSound);
        TakeDamageWithPopup(damageToTake, popupPosition, hitter, ignoreEffect);
    }

    private void TakeDamageWithPopup(float damageToTake, Vector3 popupPosition, BaseEntity hitter, bool ignoreEffect = false)
    {
        if (health > 0)
        {
            TakeDamagePureNumbers(damageToTake, hitter, ignoreEffect);

            PrefabManager.instance.SpawnNumberPopup(Mathf.RoundToInt(damageToTake), PrefabManager.instance.orange, popupPosition);
        }

        //(Vector2)transform.position + UnityEngine.Random.insideUnitCircle)
    }

    public virtual void TakeDamageNoSound(float damageToTake, Vector3 popupPosition, BaseEntity hitter, bool ignoreEffect = false)
    {
        TakeDamageWithPopup(damageToTake, popupPosition, hitter, ignoreEffect);
    }

    private void TakeDamagePureNumbers(float damageToTake, BaseEntity hitter, bool ignoreEffect = false)
    {
        if (!ignoreEffect)
        {
            GameEvents.instance.OnEntityHit(this, hitter, damageToTake);
        }      

        float damageToGive = 0;
        if (armour.Value >= 0)
        {
            damageToGive = (UnityEngine.Random.Range(damageToTake - 2, damageToTake + 2) * (100 / (100 + armour.Value)));
        }
        else
        {
            damageToGive = (UnityEngine.Random.Range(damageToTake - 2, damageToTake + 2) * (2 - 100 / (100 - armour.Value)));
        }
        health -= damageToGive;
    }

    public virtual void Die()
    {
        GameEvents.instance.OnEntityKilled(this);
        Destroy(gameObject);
    }
}
