using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    public string planetName;
    public string planetDescription;
    public bool breathableAtmosphere;
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
    public float breathingPerBillion;
    public float projectedFoodConsumptionNextTick;
    public float totalFoodReserves;

    private PrefabManager pf;
    private TradeRoutesManager tmManager;

    private void Start()
    {
        tmManager = GameObject.FindGameObjectWithTag("TMManager").GetComponent<TradeRoutesManager>();
        pf = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        allPlanets = GameObject.FindGameObjectsWithTag("Planet").Select(x => x.GetComponent<Planet>()).ToList();
        GetComponent<SpriteRenderer>().sortingOrder = -Mathf.RoundToInt(transform.position.z);

        availableTradeRoutes = new List<TradeRoute>();

        if(products.Count() == 0)
        {
            products = new List<PlanetProduction>();
        }

        if(dependencies.Count() == 0)
        {
            dependencies = new List<PlanetProduction>();
        }            

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

        totalWealth = population * commoditiesInMarket.Sum(x => x.stack);
    }

    private void Update()
    {
        totalInfluence = influenceModifier / population * totalWealth;
        
        projectedFoodConsumptionNextTick = population * foodPerBillion;
    }

    public void Tick()
    {
        HandlePopulationUsage();
        FindAllDeficits();      

        foreach (PlanetProduction prod in products)
        {
            GenerateResource(prod);
        }

        foreach (PlanetProduction item in dependencies)
        {
            SubtractResource(item);
        }                   
    }

    public void FindAllDeficits()
    {
        foreach (var item in dependencies)
        {
            if (item.lookingForTypeOnly)
            {
                if (IncomeDeficit.CalculateProfit(this, item.typeLookingFor) < 0)
                {
                    TryForNewTradeRoutes(item.typeLookingFor, Mathf.Abs(IncomeDeficit.CalculateProfit(this, item.typeLookingFor)));
                }
            }
            else
            {
                if (IncomeDeficit.CalculateProfit(this, item.comProduced) < 0)
                {
                    TryForNewTradeRoutes(item.comProduced, Mathf.Abs(IncomeDeficit.CalculateProfit(this, item.comProduced)));
                }
            }               
        }

        if(availableTradeRoutes.Count() > 0)
        {
            foreach (var item in availableTradeRoutes.Where(x => x.sender == this).Where(x => x.commodityToTransport != null))
            {
                if (IncomeDeficit.CalculateProfit(this, item.commodityToTransport) < 0)
                {
                    TryForNewTradeRoutes(item.commodityToTransport, Mathf.Abs(IncomeDeficit.CalculateProfit(this, item.commodityToTransport)));
                }
            }

            foreach (var item in availableTradeRoutes.Where(x => x.sender == this).Where(x => x.commTypeToTransport != Commodity.Type.None))
            {
                if (IncomeDeficit.CalculateProfit(this, item.commTypeToTransport) < 0)
                {
                    TryForNewTradeRoutes(item.commTypeToTransport, Mathf.Abs(IncomeDeficit.CalculateProfit(this, item.commTypeToTransport)));
                }
            }
        }           
    }

    public void HandlePopulationUsage()
    {
        List<Commodity> foodItems = commoditiesInMarket.Where(x => x.commodityType == Commodity.Type.Food).ToList();
        totalFoodReserves = foodItems.Sum(x => x.stack);

        if (population == 0) return;                  

        if(dependencies.Where(x => x.comProduced == null).Where(x => x.typeLookingFor == Commodity.Type.Food).Count() != 0)
        {
            dependencies.Where(x => x.comProduced == null).Where(x => x.typeLookingFor == Commodity.Type.Food).First().comAmountPerTick = -(foodPerBillion * population);
        }
        else
        {
            PlanetProduction prod = new PlanetProduction(Commodity.Type.Food, -(foodPerBillion * population));
            dependencies.Add(prod);
            WarningLowResource(prod.typeLookingFor.ToString());
        }

        if (breathableAtmosphere) return;

        if (dependencies.Where(x => x.comProduced == null).Where(x => x.typeLookingFor == Commodity.Type.Breathing).Count() != 0)
        {
            dependencies.Where(x => x.comProduced == null).Where(x => x.typeLookingFor == Commodity.Type.Breathing).First().comAmountPerTick = -(breathingPerBillion * population);
        }
        else
        {
            PlanetProduction prod = new PlanetProduction(Commodity.Type.Breathing, -(breathingPerBillion * population));
            dependencies.Add(prod);
            WarningLowResource(prod.typeLookingFor.ToString());
        }
    }

    public void WarningLowResource(string warning)
    {
        Debug.Log("WARNING! " + this.planetName + " has LOW " + warning.ToUpper());
    }

    public void TryForNewTradeRoutes(Commodity.Type type, float amount)
    {
        Debug.Log(planetName + " is trying to find routes for " + Mathf.Abs(amount) + " " + type.ToString() + ".");

        List<GameObject> possibilities = FindPlanetsSupplyingCommodityType(type).Select(x => x.gameObject)
            .OrderBy(x => Vector2.Distance(gameObject.transform.position, x.gameObject.transform.position)).ToList();

        possibilities.Remove(gameObject);

        foreach (var item in possibilities)
        {
            if (item == gameObject) return;

            if (RequestTradeRoute(this, item.GetComponent<Planet>(), type, Mathf.Abs(amount)))
            {
                SetUpTradeRoute(this, item.GetComponent<Planet>(), type, amount);
                return;
            }
        }
    }

    public void TryForNewTradeRoutes(Commodity comm, float amount)
    {
        Debug.Log(planetName + " is trying to find routes for " + Mathf.Abs(amount) + " " + comm.commodityName.ToString() + ".");

        List<GameObject> possibilities = FindPlanetsSupplyingCommodity(comm).Select(x => x.gameObject)
            .OrderBy(x => Vector2.Distance(gameObject.transform.position, x.gameObject.transform.position)).ToList();

        possibilities.Remove(gameObject);

        foreach (var item in possibilities.Where(x => x.GetComponent<Planet>().planetName != planetName))
        {
            if (item == gameObject) return;

            if (RequestTradeRoute(this, item.GetComponent<Planet>(), comm, Mathf.Abs(amount)))
            {
                SetUpTradeRoute(this, item.GetComponent<Planet>(), comm, amount);
                return;
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
        float spare = IncomeDeficit.CalculateProfit(giver, comm);

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
        if (asker == giver) return;

        Debug.Log(asker.planetName + " setting up trade route with " + giver.planetName + " for " + Mathf.Abs(amount) + " " + type.ToString());

        TradeRoute route = new TradeRoute(giver, asker, type, Mathf.Abs(amount));
        tmManager.AddNewRoute(route);
    }

    public void SetUpTradeRoute(Planet asker, Planet giver, Commodity comm, float amount)
    {
        if (asker == giver) return;

        Debug.Log(asker.planetName + " setting up trade route with " + giver.planetName + " for " + Mathf.Abs(amount) + " " + comm.commodityName.ToString());

        Commodity commo = (Commodity)ScriptableObject.Instantiate(comm);
        commo.ChangeToComm(comm, amount);

        TradeRoute route = new TradeRoute(giver, asker, commo, Mathf.Abs(amount));
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
            comms[0].stack -= Mathf.Abs(prod.comAmountPerTick);
        }
        catch
        {
            totalWealth -= prod.comAmountPerTick * population;
        }           
    }

    public bool SubtractResource(Commodity commToTake, float amount)
    {
        Commodity comm = commoditiesInMarket.Where(x => x.commodityName == commToTake.commodityName).First();

        if(comm.stack >= Mathf.Abs(amount))
        {
            comm.stack -= Mathf.Abs(amount);
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
        List<Planet> planets = allPlanets.Where(x => x.products.
        Where(x => x.comProduced.commodityName == comToSearch.commodityName).ToList().Count >= 0).ToList();

        return planets;
    }

    public List<Planet> FindPlanetsSupplyingCommodityType(Commodity.Type type)
    {
        List<Planet> planets = allPlanets.Where(x => x.products.
        Where(x => x.comProduced.commodityType == type).ToList().Count >= 0).ToList();

        return planets;
    }
}
