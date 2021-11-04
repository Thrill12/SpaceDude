using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public string planetName;
    public string planetDescription;
    public float totalInfluence;
    //In Billion
    public long population;
    public float totalWealth;
    public float influenceModifier;
    public List<Commodity> startingComms;
    public List<Commodity> commoditiesInMarket;
    public List<TradeRoute> availableTradeRoutes;
    public List<PlanetProduction> products;
    public List<PlanetProduction> dependencies;
    public List<Planet> allPlanets;

    [Space(5)]

    [Header("Population Usage")]

    public float foodPerBillion = 1;
    public float projectedFoodConsumptionNextTick;
    public float totalFoodReserves;

    [Space(5)]

    private float planetTick = 5;
    private float nextTick = 0;
    private PrefabManager pf;
    private TradeRoutesManager tmManager;

    private void Start()
    {
        tmManager = GameObject.FindGameObjectWithTag("TMManager").GetComponent<TradeRoutesManager>();

        if(products.Count() == 0)
        {
            products = new List<PlanetProduction>();
        }

        if(dependencies.Count() == 0)
        {
            dependencies = new List<PlanetProduction>();
        }

        GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.position.z);
        pf = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        allPlanets = GameObject.FindGameObjectsWithTag("Planet").Select(x => x.GetComponent<Planet>()).ToList();

        foreach (var item in startingComms)
        {
            commoditiesInMarket.Add((Commodity)ScriptableObject.Instantiate(item));
        }

        foreach (PlanetProduction item in dependencies)
        {
            GenerateResource(new PlanetProduction(item.comProduced, Mathf.Abs(item.comAmountPerTick) * 2f));
        }

        foreach (PlanetProduction item in products)
        {
            GenerateResource(new PlanetProduction(item.comProduced, Mathf.Abs(item.comAmountPerTick) * 10f));
        }

        Tick();
    }

    private void Update()
    {
        if (nextTick <= 0)
        {
            nextTick = planetTick;
            Tick();
        }

        totalInfluence = influenceModifier * population;

        nextTick -= Time.deltaTime;

        projectedFoodConsumptionNextTick = population * foodPerBillion;
    }

    public void Tick()
    {
        foreach (PlanetProduction prod in products)
        {
            GenerateResource(prod);
        }

        foreach (PlanetProduction item in dependencies)
        {
            SubtractResource(item);
        }

        HandlePopulationUsage();
    }

    public void HandlePopulationUsage()
    {
        List<Commodity> foodItems = commoditiesInMarket.Where(x => x.commodityType == Commodity.Type.Food).ToList();
        totalFoodReserves = foodItems.Sum(x => x.stack);

        float foodNet = IncomeDeficit.CalculateProfit(this, Commodity.Type.Food);

        if (population == 0) return;             
        
        if(foodNet < 0)
        {
            WarningLowFood();
            TryForNewTradeRoutes(Commodity.Type.Food, foodNet);
        }        

        if(dependencies.Where(x => x.typeLookingFor == Commodity.Type.Food).Count() != 0)
        {
            dependencies.Where(x => x.typeLookingFor == Commodity.Type.Food).First().comAmountPerTick = -(foodPerBillion * population);
        }
        else
        {
            dependencies.Add(new PlanetProduction(Commodity.Type.Food, -(foodPerBillion * population)));
        }
    }

    public void WarningLowFood()
    {
        Debug.Log("WARNING! " + this.planetName + " has LOW FOOD");
    }

    public void TryForNewTradeRoutes(Commodity.Type type, float amount)
    {
        Debug.Log(planetName + " is trying to find routes for " + amount + " " + type.ToString() + ".");

        List<GameObject> possibilities = FindPlanetsSupplyingCommodityType(type).Select(x => x.gameObject).OrderBy(x => Vector2.Distance(gameObject.transform.position, x.gameObject.transform.position)).ToList();

        possibilities.Remove(gameObject);

        foreach (var item in possibilities)
        {
            if(RequestTradeRoute(this, item.GetComponent<Planet>(), type, amount))
            {
                SetUpTradeRoute(this, item.GetComponent<Planet>(), type, amount);
            }
        }
    }

    public bool RequestTradeRoute(Planet asker, Planet giver, Commodity.Type type, float amountRequested)
    {
        Debug.Log(asker.planetName + " asking for trade route with " + giver.planetName + " for " + amountRequested + " " + type.ToString());
        float spare = IncomeDeficit.CalculateProfit(giver, type);

        if (spare > 0)
        {
            if (spare > amountRequested)
            {
                Debug.Log("Request accepted");
                return true;
            }
            else
            {
                Debug.Log("Request denied - spare less than requested");
                return false;
            }
        }
        else
        {
            Debug.Log("Request denied - no spare");
            return false;
        }
    }

    public bool RequestTradeRoute(Planet asker, Planet giver, Commodity comm, float amountRequested)
    {
        Debug.Log(asker.planetName + " asking for trade route with " + giver.planetName + " for " + Mathf.Abs(amountRequested) + " " + comm.commodityName);
        float spare = IncomeDeficit.CalculateProfit(giver, Commodity.Type.Food, comm);

        if (spare > 0)
        {
            if (spare > amountRequested)
            {
                Debug.Log("Request accepted");
                return true;
            }
            else
            {
                Debug.Log("Request denied - spare less than requested");
                return false;
            }
        }
        else
        {
            Debug.Log("Request denied - no spare");
            return false;
        }
    }

    public void SetUpTradeRoute(Planet asker, Planet giver, Commodity.Type type, float amount)
    {
        Debug.Log(asker.planetName + " setting up trade route with " + giver.planetName + " for " + Mathf.Abs(amount) + " " + type.ToString());

        Commodity comm = giver.commoditiesInMarket.Where(x => x.commodityType == type).OrderBy(x => x.stack).First();

        //PlanetProduction prod = new PlanetProduction(type, Mathf.Abs(amount));
        //prod.originDestination = giver;
        //prod.comProduced = comm;
        //asker.receivingFromPlanets.Add(prod);

        //prod = new PlanetProduction(type, amount);
        //prod.originDestination = asker;
        //prod.comProduced = comm;
        //giver.sendingToPlanets.Add(prod);

        TradeRoute route = new TradeRoute(giver, asker, comm, amount);
        tmManager.AddNewRoute(route);
    }

    public void GenerateResource(PlanetProduction prod)
    {
        if (commoditiesInMarket.Where(x => x.commodityName == prod.comProduced.commodityName).ToList().Count != 0)
        {
            commoditiesInMarket.Where(x => x.commodityName == prod.comProduced.commodityName).First().stack += prod.comAmountPerTick;
        }
        else
        {
            commoditiesInMarket.Add((Commodity)ScriptableObject.Instantiate(prod.comProduced));
            commoditiesInMarket.Where(x => x.commodityName == prod.comProduced.commodityName).First().stack += prod.comAmountPerTick;
        }
    }

    public void SubtractResource(PlanetProduction prod)
    {
        try
        {
            List<Commodity> comms = commoditiesInMarket.Where(x => x.commodityType == prod.typeLookingFor).ToList();
            comms[0].stack += prod.comAmountPerTick;
        }
        catch
        {
            Debug.Log("SOMETHING WRONG IN SUBTRACT RESOURCE");
        }           
    }

    public bool SubtractResource(Commodity commToTake, float amount)
    {
        Commodity comm = commoditiesInMarket.Where(x => x.commodityName == commToTake.commodityName).First();

        if(comm.stack >= amount)
        {
            comm.stack -= amount;
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ReceiveCommodity(Commodity comm)
    {
        if (commoditiesInMarket.Where(x => x.commodityName == comm.commodityName).Count() != 0)
        {
            commoditiesInMarket.Where(x => x.commodityName == comm.commodityName).First().stack += comm.stack;
        }
        else
        {
            commoditiesInMarket.Add(comm);            
        }
    }  

    public List<Planet> FindPlanetsSupplyingCommodity(Commodity comToSearch)
    {
        return allPlanets.Where(x => x.products.Where(x => x.comProduced.commodityName == comToSearch.commodityName).ToList().Count != 0).ToList();
    }

    public List<Planet> FindPlanetsSupplyingCommodityType(Commodity.Type type)
    {
        return allPlanets.Where(x => x.products.Where(x => x.comProduced.commodityType == type).ToList().Count != 0).ToList();
    }
}
