using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntity : BaseEntity
{
    public Image healthDisplay;
    public Image energyDisplay;

    [Space(5)]

    public float oxygen;
    public Stat maxOxygen;
    public Image oxygenDisplay;

    [HideInInspector]
    public bool useOxygen = false;
    private float nextOxygenTick;

    public override void Start()
    {
        base.Start();

        oxygen = maxOxygen.Value;
    }

    public override void Update()
    {
        base.Update();
        healthDisplay.fillAmount = Mathf.Lerp(healthDisplay.fillAmount, health / maxHealth.Value, 0.5f);
        energyDisplay.fillAmount = Mathf.Lerp(energyDisplay.fillAmount, energy / maxEnergy.Value, 0.5f);
        oxygenDisplay.fillAmount = Mathf.Lerp(oxygenDisplay.fillAmount, oxygen / maxOxygen.Value, 0.5f);

        if (useOxygen)
        {
            nextOxygenTick -= Time.deltaTime;

            if (nextOxygenTick <= 0)
            {
                if (oxygen < 0)
                {
                    TakeDamagePure(oxygen);
                    if (!UIManager.instance.isWarning)
                    {
                        UIManager.instance.ShowWarning("Low Oxygen");
                    }
                }

                oxygen -= 1;
                nextOxygenTick = 1;
            }            
        }
        else
        {
            oxygen = maxOxygen.Value;
        }
    }
}
