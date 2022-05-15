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
    public Stat moveSpeed;

    [Space(5)]

    public AudioClip hitSound;

    float nextFireHealthRegen;
    float nextFireEnergyRegen;

    [HideInInspector]
    public PrefabManager pfMan;

    [HideInInspector]
    public bool isDead;

    private GameEvents gameEvents;

    [HideInInspector]
    public List<Modifier> itemSetModifiers;

    private void Awake()
    {
        health = maxHealth.Value;
    }

    public virtual void Start()
    {
        gameEvents = GameObject.FindGameObjectWithTag("GameEvents").GetComponent<GameEvents>();
        pfMan = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        gameEvents.onEntityKilled += OnKill;
    }

    public virtual void Update()
    {     
        if (nextFireHealthRegen <= 0)
        {
            RegenHealth(healthRegeneration.Value);
        }

        if (nextFireEnergyRegen <= 0)
        {
            RegenEnergy(energyRegeneration.Value);
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
        energy += energyToRegen;
        nextFireEnergyRegen = 1;
    }

    public virtual void TakeDamage(float damageToTake, Vector3 popupPosition, BaseEntity hitter, bool ignoreEffect = false, EffectToCheck effectFlags = 0)
    {
        GetComponent<AudioSource>().PlayOneShot(hitSound);
        TakeDamageWithPopup(damageToTake, popupPosition, hitter, ignoreEffect, effectFlags);
    }

    private void TakeDamageWithPopup(float damageToTake, Vector3 popupPosition, BaseEntity hitter, bool ignoreEffect = false, EffectToCheck effectFlags = 0)
    {
        if (health > 0)
        {
            TakeDamagePureNumbers(damageToTake, hitter, ignoreEffect, effectFlags);

            PrefabManager.instance.SpawnNumberPopup(Mathf.RoundToInt(damageToTake), PrefabManager.instance.ascendedColour, popupPosition);
        }

        //(Vector2)transform.position + UnityEngine.Random.insideUnitCircle)
    }

    public virtual void TakeDamageNoSound(float damageToTake, Vector3 popupPosition, BaseEntity hitter, bool ignoreEffect = false, EffectToCheck effectFlags = 0)
    {
        TakeDamageWithPopup(damageToTake, popupPosition, hitter, ignoreEffect, effectFlags);
    }

    private void TakeDamagePureNumbers(float damageToTake, BaseEntity hitter, bool ignoreEffect = false, EffectToCheck effectFlags = 0)
    {
        if (!ignoreEffect)
        {
            gameEvents.OnEntityHit(this, hitter, damageToTake, effectFlags);
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

        if (health <= 0 && !isDead)
        {
            Die(hitter);
            isDead = true;
        }
    }

    public void TakeDamagePure(float damage)
    {
        health -= damage;
        PrefabManager.instance.SpawnNumberPopup(Mathf.RoundToInt(damage), PrefabManager.instance.ascendedColour, transform.position + (Vector3)UnityEngine.Random.insideUnitCircle);
    }

    public virtual void Die(BaseEntity killer)
    {
        gameEvents.OnEntityKilled(this, killer);
        Destroy(gameObject);
    }

    public virtual void OnKill(BaseEntity victim, BaseEntity killer)
    {
        if(killer == this)
        {
            Debug.Log(entityName + " killed " + victim.entityName);
        }       
    }
}
