using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TradeRoutesManager : MonoBehaviour
{
    public List<TradeRoute> allRoutes;
    [Header("Cargo Ship Speed Curve"), Tooltip("The 0-1 value is a perecentage of jopurney completion, so 0.5 = 50% complete. Speed should be an inveerted U shape.")]
    public AnimationCurve speedCurve;

    public PrefabManager pf;

    //Spawns a trade ship on each tick
    public void TickAllTradeRoutes()
    {
        foreach (var item in allRoutes)
        {
            SpawnNewShip(item.sender, item.receiver, item);
        }
    }

    //Sets up and spawns ship for a new route from a given route created
    public void AddNewRoute(TradeRoute route)
    {
        allRoutes.Add(route);
        route.sender.availableTradeRoutes.Add(route);
        route.receiver.availableTradeRoutes.Add(route);
        SpawnNewShip(route.sender, route.receiver, route);
    }

    //Spawn a new ship for trading between planets
    public void SpawnNewShip(Planet sender, Planet receiver, TradeRoute route)
    {
        //if(route.itemToTransport != null)
        //{
        //    Debug.Log(route.sender + " -> " + route.receiver + " | " + route.itemToTransport.itemName + " |");
        //}
        //else
        //{
        //    Debug.Log(route.sender + " -> " + route.receiver + " | " + route.itemTypeToTransport.ToString() + " |");
        //}

        GameObject ship = Instantiate(pf.cargoShip, sender.gameObject.transform.position, Quaternion.identity);
        ship.transform.SetParent(gameObject.transform);

        CargoShip s = ship.GetComponent<CargoShip>();

        s.origin = sender.gameObject;
        s.destination = receiver.gameObject;
        s.speedCurve = speedCurve;

        if(route.itemToTransport != null)
        {
            s.commodityCarried = route.itemToTransport;
        }
        else
        {
            s.commodityCarried = route.sender.commoditiesInMarket.Where(x => x.itemType == route.itemTypeToTransport).First();
            s.commodityCarried.itemStack = route.amount;
        }
             
    }
}
