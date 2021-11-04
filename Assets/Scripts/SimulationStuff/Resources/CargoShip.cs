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

    private Rigidbody2D rb;

    private Vector2 origin2D;
    private Vector2 dest2D;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        StartMovingToPlanet(destination.GetComponent<Planet>());
        dest2D = new Vector2(destination.transform.position.x, destination.transform.position.y);
    }

    private void Update()
    {
        Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);

        if(pos2D == dest2D)
        {
            rb.velocity *= 0;
        }        
    }

    public void StartMovingToPlanet(Planet planet)
    {
        Transform target = planet.gameObject.transform;

        rb.velocity = (target.position - transform.position).normalized * speed;
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
}
