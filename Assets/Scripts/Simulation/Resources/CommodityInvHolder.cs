using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CommodityInvHolder : MonoBehaviour
{
    public Commodity commodityHeld;

    private UIManager ui;

    private void Start()
    {
        ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }

    private void Update()
    {
        transform.Find("Commodity Units").GetComponent<TMP_Text>().text = commodityHeld.stack + " units";
    }

    public void SelectCommodity()
    {
        ui.MarketSelectCommodityInMarketWindow(commodityHeld);
    }
}
