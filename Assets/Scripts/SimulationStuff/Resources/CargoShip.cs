using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CargoShip : MonoBehaviour
{
    public float speed;

    public Commodity commodityCarried;
    public GameObject origin;
    public GameObject destination;

    public float progressCount;
    public float routeDistance;
    public float travelledDistance;
    public float planetRadiusFloat;

    private Rigidbody2D rb;

    private Vector2 origin2D;
    private Vector2 dest2D;
    public bool hasDroppedCargo = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        
        StartMovingToPlanet(destination.GetComponent<Planet>());
        dest2D = new Vector2(destination.transform.position.x, destination.transform.position.y);

        routeDistance = Vector2.Distance(transform.position, dest2D);
        planetRadiusFloat = destination.GetComponent<SpriteRenderer>().bounds.extents.x / routeDistance;
    }

    private void Update()
    {      
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);

        travelledDistance = Vector2.Distance(pos2D, origin.transform.position);
        progressCount = travelledDistance / routeDistance;

        

        if (pos2D == dest2D)
        {
            rb.velocity *= 0;
        }

        StartMovingToPlanet(destination.GetComponent<Planet>());

        if(progressCount > 1 - planetRadiusFloat && !hasDroppedCargo)
        {
            DropCargo();
        }
    }

    public void StartMovingToPlanet(Planet planet)
    {
        Transform target = planet.gameObject.transform;

        rb.velocity = (target.position - transform.position) * speed;
        transform.up = -rb.velocity.normalized;
    }

    public void TakeCargo(Planet planet, Commodity commToTake, float stackToTake)
    {
        Commodity comm = planet.commoditiesInMarket.Where(x => x.commodityName == commToTake.commodityName).First();
        if (comm.stack >= stackToTake)
        {
            if(planet.SubtractResource(commToTake, stackToTake))
            {
                commodityCarried = new Commodity(commToTake, stackToTake);              
            }
        }
    }

    public void DropCargo()
    {
        hasDroppedCargo = true;
        destination.GetComponent<Planet>().ReceiveCommodity(commodityCarried);
        GetComponent<Animator>().SetTrigger("GetOutOfOrbit");
        Destroy(gameObject, 2);
    }
}
