using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHits : MonoBehaviour
{
    public static EntityHits instance;

    public GameObject bleedEffect;

    private void Awake()
    {
        instance = this;       
    }

    private void Start()
    {
        GameEvents.instance.onEntityHit += EntityHit;
    }

    public void EntityHit(BaseEntity victim, BaseEntity hitter, float damage)
    {
        CheckForStatusEffects(victim, hitter, damage);
        Debug.Log(hitter.entityName + " hit " + victim.entityName);
    }

    public void CheckForStatusEffects(BaseEntity victim, BaseEntity hitter, float damage)
    {
        CheckForBleed(victim, hitter);
        CheckForPoison(victim, hitter);
        CheckForCrit(victim, hitter);
    }

    public void CheckForBleed(BaseEntity victim, BaseEntity hitter)
    {
        if (victim.GetComponent<Bleed>())
        {
            victim.GetComponent<Bleed>().source = hitter;
            victim.GetComponent<Bleed>().Reset();
            victim.GetComponent<Bleed>().stack += 1;
        }
        else
        {
            victim.gameObject.AddComponent<Bleed>();
            victim.gameObject.GetComponent<Bleed>().effectVisualization = bleedEffect;
            victim.gameObject.GetComponent<Bleed>().source = hitter;
        }
    }

    public void CheckForPoison(BaseEntity victim, BaseEntity hitter)
    {
        Debug.Log("Checking poison");
    }

    public void CheckForCrit(BaseEntity victim, BaseEntity hitter)
    {
        Debug.Log("Checking crit");
    }
}
