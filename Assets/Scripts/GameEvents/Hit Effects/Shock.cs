using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shock : HitEffect
{
    public float damage = 2;
    public float moveSpeedmodifier = -5;

    public override void OnEffect()
    {
        GetComponent<BaseEntity>().TakeDamage(damage * stack * source.damageMultiplier.Value, (Vector2)transform.position + Random.insideUnitCircle, source, true);
        Modifier speedModifier = new Modifier("Shock Slowness", GetComponent<BaseEntity>().moveSpeed, moveSpeedmodifier, Modifier.StatModType.PercentAdd);
        speedModifier.Source = this;
        GetComponent<BaseEntity>().moveSpeed.AddModifier(speedModifier);
    }

    public override void OnDestroy()
    {
        GetComponent<BaseEntity>().moveSpeed.RemoveAllModifiersFromSource(this);
        base.OnDestroy();
    }
}
