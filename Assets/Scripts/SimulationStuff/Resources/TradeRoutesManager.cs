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

    public void AddNewRoute(TradeRoute route)
    {
        allRoutes.Add(route);
        route.sender.availableTradeRoutes.Add(route);
        route.receiver.availableTradeRoutes.Add(route);
        SpawnNewShip(route.sender, route.receiver, route);
    }

    public void SpawnNewShip(Planet origin, Planet destination, TradeRoute route)
    {
        GameObject ship = Instantiate(pf.cargoShip, origin.transform.position, Quaternion.identity);
        ship.transform.position = new Vector3(ship.transform.position.x, ship.transform.position.y, 0);

        CargoShip s = ship.GetComponent<CargoShip>();

        s.origin = origin.gameObject;
        s.destination = destination.gameObject;
        s.commodityCarried = route.commodityToTransport;
    }
}
