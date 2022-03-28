using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHits : MonoBehaviour
{
    public static EntityHits instance;

    public GameObject bleedEffect;
    public GameObject shockEffect;

    private GameEvents gameEvents;

    private void Awake()
    {
        instance = this;       
    }

    private void OnLevelWasLoaded(int level)
    {
        try
        {
            gameEvents = GameObject.FindGameObjectWithTag("GameEvents").GetComponent<GameEvents>();
            gameEvents.onEntityHit += EntityHit;
        }
        catch
        {
            Debug.Log("No game events could be found");
        }     
    }

    public void EntityHit(BaseEntity victim, BaseEntity hitter, float damage, EffectToCheck effectFlags)
    {
        CheckForSpecificEffects(victim, hitter, damage, effectFlags);
        Debug.Log(hitter.entityName + " hit " + victim.entityName);
    }

    public void CheckForBleed(BaseEntity victim, BaseEntity hitter)
    {
        CheckForEffectComponent<Bleed>(victim, hitter, bleedEffect);
    }        

    public void CheckForShock(BaseEntity victim, BaseEntity hitter)
    {
        CheckForEffectComponent<Shock>(victim, hitter, shockEffect);
    }

    public void CheckForPoison(BaseEntity victim, BaseEntity hitter)
    {
        Debug.Log("Checking poison");
    }

    public void CheckForCrit(BaseEntity victim, BaseEntity hitter)
    {
        Debug.Log("Checking crit");
    }

    private void CheckForEffectComponent<T>(BaseEntity victim, BaseEntity hitter, GameObject visualization) where T : HitEffect
    {
        if (victim.GetComponent<T>())
        {
            victim.GetComponent<T>().source = hitter;
            victim.GetComponent<T>().Reset();
            victim.GetComponent<T>().stack += 1;
        }
        else
        {
            victim.gameObject.AddComponent<T>();
            victim.gameObject.GetComponent<T>().effectVisualization = visualization;
            victim.gameObject.GetComponent<T>().source = hitter;
        }
    }

    public void CheckForSpecificEffects(BaseEntity victim, BaseEntity hitter, float damage, EffectToCheck effectFlags)
    {
        if((effectFlags & EffectToCheck.bleed) != 0)
        {
            CheckForBleed(victim, hitter);
        }

        if((effectFlags & EffectToCheck.poison) != 0)
        {
            CheckForPoison(victim, hitter);
        }

        if ((effectFlags & EffectToCheck.crit) != 0)
        {
            CheckForCrit(victim, hitter);
        }

        if ((effectFlags & EffectToCheck.shock) != 0)
        {
            CheckForShock(victim, hitter);
        }
    }
}

[System.Flags]
public enum EffectToCheck : int
{
    none = 1<<0,
    bleed = 1<<1,
    poison = 1<<2,
    crit = 1<<3,
    shock = 1<<4,
}
