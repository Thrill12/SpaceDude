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

        if (Input.GetMouseButtonDown(0))
        {
            Vector3 mousePoss = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePoss2D = new Vector2(mousePoss.x, mousePoss.y);

            RaycastHit2D hitt = Physics2D.Raycast(mousePoss2D, Vector2.zero);

            if (hitt != false)
            {
                if (hitt.collider.CompareTag("Item"))
                {
                    hitt.collider.gameObject.GetComponent<ItemHolder>().ClickedOn(gameObject);
                }
            }
        }
    }
}
