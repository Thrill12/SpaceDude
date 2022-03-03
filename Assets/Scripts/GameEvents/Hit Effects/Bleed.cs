using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bleed : HitEffect
{
    public float damage = 5;

    public override void OnEffect()
    {
        GetComponent<BaseEntity>().TakeDamage(damage * stack * source.damageMultiplier.Value, (Vector2)transform.position + Random.insideUnitCircle, source, true);
    }
}
