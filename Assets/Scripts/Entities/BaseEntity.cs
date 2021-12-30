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
    public Stat energyRegeneration;

    private void Start()
    {
        health = maxHealth.Value;
    }
}
