using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TradeRoutesManager : MonoBehaviour
{
    public List<TradeRoute> allRoutes;

    private PrefabManager pf;

    private void Start()
    {
        pf = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
    }

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
        GameObject ship = Instantiate(pf.cargoShip, sender.gameObject.transform.position, Quaternion.identity);
        ship.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, 0);

        CargoShip s = ship.GetComponent<CargoShip>();

        s.origin = sender.gameObject;
        s.destination = receiver.gameObject;
        s.commodityCarried = route.commodityToTransport;
    }
}
