using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TickManager : MonoBehaviour
{
    public float tickCooldown = 10;

    public TradeRoutesManager trManager;

    private float nextTick = 1;
    private List<Planet> allPlanets;

    private void Start()
    {
        allPlanets = GameObject.FindGameObjectsWithTag("Planet").Select(x => x.GetComponent<Planet>()).ToList();
    }

    private void Update()
    {
        if(Time.deltaTime > nextTick)
        {
            nextTick = Time.deltaTime + tickCooldown;
            TickAllPlanets();
        }

        nextTick -= Time.deltaTime;
    }

    public void TickAllPlanets()
    {
        foreach (var item in allPlanets)
        {
            item.Tick();
        }
        trManager.TickAllTradeRoutes();
    }
}
