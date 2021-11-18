using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TradeRoutesManager : MonoBehaviour
{
    public List<TradeRoute> allRoutes;

    public PrefabManager pf;

    public void TickAllTradeRoutes()
    {
        foreach (var item in allRoutes)
        {
            SpawnNewShip(item.sender, item.receiver, item);
        }
    }

    public void AddNewRoute(TradeRoute route)
    {
        allRoutes.Add(route);
        route.sender.availableTradeRoutes.Add(route);
        route.receiver.availableTradeRoutes.Add(route);
        SpawnNewShip(route.sender, route.receiver, route);
    }

    public void SpawnNewShip(Planet sender, Planet receiver, TradeRoute route)
    {
        if(route.commodityToTransport != null)
        {
            Debug.Log(route.sender + " -> " + route.receiver + " | " + route.commodityToTransport.commodityName + " |");
        }
        else
        {
            Debug.Log(route.sender + " -> " + route.receiver + " | " + route.commTypeToTransport.ToString() + " |");
        }

        GameObject ship = Instantiate(pf.cargoShip, sender.gameObject.transform.position, Quaternion.identity);
        ship.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, 0);

        CargoShip s = ship.GetComponent<CargoShip>();

        s.origin = sender.gameObject;
        s.destination = receiver.gameObject;

        if(route.commodityToTransport != null)
        {
            s.commodityCarried = route.commodityToTransport;
        }
        else
        {
            s.commodityCarried = route.sender.commoditiesInMarket.Where(x => x.commodityType == route.commTypeToTransport).First();
            s.commodityCarried.stack = route.amount;
        }
             
    }
}
