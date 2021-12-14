using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CargoShip : MonoBehaviour
{
    public enum FlightState { Launching, Travelling, Landing};

    [Header("Ship Flight Settings")]
    public FlightState flightState;
    [Tooltip("This is the layer the ship should be on when on take off/landing (the one rendered by the background camera.)")]
    public string perspectiveMask;
    [Tooltip("This is the layer the ship should be on when travelling (the one rendered by the main camera.)")]
    public string orthoMask;
    public float launchSpeed;

    public Commodity commodityCarried;
    public GameObject origin;
    public GameObject destination;

    [Header("Speed Curve"), Tooltip("The 0-1 value is a perecentage of jopurney completion, so 0.5 = 50% complete. Speed should be an inveerted U shape.")] 
    public AnimationCurve speedCurve;

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
        
        //StartMovingToPlanet(destination.GetComponent<Planet>());
        dest2D = new Vector2(destination.transform.position.x, destination.transform.position.y);
        routeDistance = Vector2.Distance(transform.position, dest2D);
        planetRadiusFloat = destination.GetComponent<SpriteRenderer>().bounds.extents.x / routeDistance;

        //Sets the objects layer to that of the perspective mask.
        gameObject.layer = LayerMask.NameToLayer(perspectiveMask);
        //On spawn the ship would be taking off from a body - set the FlightState to Launching.
        flightState = FlightState.Launching;
    }

    private void Update()
    {
        float zDist;

        switch (flightState)
        { 

            case FlightState.Launching:
                zDist = (0 - transform.position.z);
                if (zDist < 0) { zDist = -(zDist); }

                if (zDist > 5)
                {
                    LaunchShip();
                }
                else
                {
                    transform.position = new Vector2(transform.position.x, transform.position.y);
                    flightState = FlightState.Travelling;
                    gameObject.layer = LayerMask.NameToLayer(orthoMask);
                }
                break;
            case FlightState.Travelling:
                Vector2 pos2D = new Vector2(transform.position.x, transform.position.y);

                travelledDistance = Vector2.Distance(pos2D, origin.transform.position);
                progressCount = travelledDistance / routeDistance;

                StartMovingToPlanet(destination.GetComponent<Planet>());

                if ((Vector2.Distance(pos2D, dest2D) < 3) && !hasDroppedCargo)
                {
                    rb.velocity *= 0;
                    flightState = FlightState.Landing;
                    gameObject.layer = LayerMask.NameToLayer(perspectiveMask);
                }
                break;
            case FlightState.Landing:
                 zDist = (destination.transform.position.z - transform.position.z);
                if (zDist < 0) { zDist = -(zDist); }

                if ( zDist > 5)
                {
                    LandShip();
                }     
                else
                {
                    DropCargo();
                }
                break;
            default:
                break;
        }
    }

    public void StartMovingToPlanet(Planet planet)
    {
        Transform target = planet.gameObject.transform;

        rb.velocity = (target.position - transform.position) * speedCurve.Evaluate(progressCount);
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

        flightState = FlightState.Launching;
    }

    public void DropCargo()
    {
        hasDroppedCargo = true;
        destination.GetComponent<Planet>().ReceiveCommodity(commodityCarried);
        GetComponent<Animator>().SetTrigger("GetOutOfOrbit");
        Destroy(gameObject, 2);
    }

    private void LaunchShip()
    {
        // Move our position a step closer to the target.
        float step = launchSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, 0), step);
    }

    private void LandShip()
    {
        // Move our position a step closer to the target.
        float step = launchSpeed * Time.deltaTime; // calculate distance to move
        transform.position = Vector3.MoveTowards(transform.position, new Vector3(destination.transform.position.x, destination.transform.position.y, destination.transform.position.z), step);
    }
}
