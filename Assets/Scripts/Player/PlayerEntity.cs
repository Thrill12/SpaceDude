using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEntity : BaseEntity
{
    public Image healthDisplay;
    public Image energyDisplay;

    public override void Start()
    {
        base.Start();
    }

    public override void Update()
    {
        base.Update();
        healthDisplay.fillAmount = health / maxHealth.Value;
        energyDisplay.fillAmount = energy / maxEnergy.Value;
    }
}
