using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class HitEffect : MonoBehaviour
{
    public int stack = 1;
    public float interval = 0.4f;

    public GameObject effectVisualization;

    public BaseEntity source;

    private float nextDamage = 0;

    public float timer = 2;
    private float nextFire = 0;

    private void Start()
    {
        nextFire = timer;

        if(GetComponent<BaseEntity>() as PlayerEntity)
        {
            PlayerEntity player = GetComponent<BaseEntity>() as PlayerEntity;
            GameObject effect = Instantiate(effectVisualization, UIManager.instance.playerHitEffectHolder.transform);
            effectVisualization = effect;
        }
        else
        {
            BaseEnemy enemy = GetComponent<BaseEntity>() as BaseEnemy;
            GameObject effect = Instantiate(effectVisualization, enemy.healthBar.gameObject.transform.parent.transform.Find("EffectHolder").transform);
            effectVisualization = effect;
        }
    }

    private void Update()
    {
        effectVisualization.GetComponentInChildren<TMP_Text>().text = "x" + stack;

        HandleEffectTiming();

        nextFire -= Time.deltaTime;

        if(nextFire <= 0)
        {
            Destroy(effectVisualization);
            Destroy(this);
        }
    }

    public void Reset()
    {
        nextFire = timer;
    }

    private void HandleEffectTiming()
    {
        if (nextDamage <= 0)
        {
            nextDamage = interval;
            OnEffect();
        }

        nextDamage -= Time.deltaTime;
    }

    public abstract void OnEffect();
}
