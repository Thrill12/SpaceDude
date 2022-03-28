using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public abstract class HitEffect : MonoBehaviour
{
    public int stack = 1;
    public float interval = 0.4f;

    public GameObject effectVisualization;

    private GameObject instantiatedVisualiation;

    public BaseEntity source;

    private float nextDamage = 0;

    public float timer = 2;
    private float nextFire = 0;

    private UIManager uiManager;

    private void Start()
    {
        nextFire = timer;

        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        if(GetComponent<BaseEntity>() as PlayerEntity)
        {
            PlayerEntity player = GetComponent<BaseEntity>() as PlayerEntity;
            instantiatedVisualiation = Instantiate(effectVisualization, uiManager.playerHitEffectHolder.transform);
        }
        else
        {
            BaseEnemy enemy = GetComponent<BaseEntity>() as BaseEnemy;
            instantiatedVisualiation = Instantiate(effectVisualization, enemy.healthBar.gameObject.transform.parent.transform.Find("EffectHolder").transform);
        }
    }

    private void Update()
    {
        effectVisualization.GetComponentInChildren<TMP_Text>().text = "x" + stack;

        HandleEffectTiming();

        nextFire -= Time.deltaTime;

        if(nextFire <= 0)
        {
            OnDestroy();
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

    public virtual void OnDestroy()
    {
        Destroy(instantiatedVisualiation);
        Destroy(this);
    }
}
