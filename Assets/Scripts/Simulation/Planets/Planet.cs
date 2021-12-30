using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [HideInInspector]
    public string planetName;
    public string planetDescription;
    public bool breathableAtmosphere;
    public bool isTakenOverByAliens;
    public float totalInfluence;
    //In Billion
    public long population;
    public float totalWealth;
    public float influenceModifier;
    public List<BaseItem> startingComms;
    public List<BaseItem> commoditiesInMarket;
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

    private void Awake()
    {
        planetName = gameObject.name;
        //Get required references
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
            commoditiesInMarket.Add(ScriptableObject.Instantiate(item));
        }

        foreach (PlanetProduction item in dependencies)
        {
            GenerateResource(new PlanetProduction(item.comProduced, Mathf.Abs(item.comAmountPerTick) * 2));
        }

        foreach (PlanetProduction item in products)
        {
            GenerateResource(new PlanetProduction(item.comProduced, Mathf.Abs(item.comAmountPerTick) * 10));
        }

        foreach (var item in commoditiesInMarket)
        {
            if (typeof(GeneralItem).IsAssignableFrom(item.GetType()))
            {
                GeneralItem genItem = (GeneralItem)item;
                totalWealth += genItem.itemStack * genItem.itemValue;
            }
            else
            {
                totalWealth += item.itemValue;
            }
        }
    }

    //On the planet's own update, update influence and project food consumpton in the next tick.
    private void Update()
    {
        totalInfluence = influenceModifier / population * totalWealth;
        
        projectedFoodConsumptionNextTick = population * foodPerBillion;
    }

    //Called by the global TickManager - updates the planet's economey.
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
                    WarningLowResource(item.typeLookingFor.ToString());
                }
            }
            else
            {
                if (IncomeDeficit.CalculateProfit(this, item.comProduced) < 0)
                {
                    TryForNewTradeRoutes(item.comProduced, Mathf.Abs(IncomeDeficit.CalculateProfit(this, item.comProduced)));
                    WarningLowResource(item.comProduced.itemName);
                }
            }               
        }

        if(availableTradeRoutes.Count() > 0)
        {
            foreach (var item in availableTradeRoutes.Where(x => x.sender == this).Where(x => x.itemToTransport != null))
            {
                if (IncomeDeficit.CalculateProfit(this, item.itemToTransport) < 0)
                {
                    TryForNewTradeRoutes(item.itemToTransport, Mathf.Abs(IncomeDeficit.CalculateProfit(this, item.itemToTransport)));
                    WarningLowResource(item.itemToTransport.itemName);
                }
            }

            foreach (var item in availableTradeRoutes.Where(x => x.sender == this).Where(x => x.itemTypeToTransport != ItemType.None))
            {
                if (IncomeDeficit.CalculateProfit(this, item.itemTypeToTransport) < 0)
                {
                    TryForNewTradeRoutes(item.itemTypeToTransport, Mathf.Abs(IncomeDeficit.CalculateProfit(this, item.itemTypeToTransport)));
                    WarningLowResource(item.itemToTransport.itemName);
                }
            }
        }           
    }

    public void HandlePopulationUsage()
    {
        List<BaseItem> foodItems = commoditiesInMarket.Where(x => x.itemType == ItemType.Food).ToList();
        totalFoodReserves = foodItems.Sum(x => x.itemStack);

        if (population == 0) return;                  

        if(dependencies.Where(x => x.comProduced == null).Where(x => x.typeLookingFor == ItemType.Food).Count() != 0)
        {
            dependencies.Where(x => x.comProduced == null).Where(x => x.typeLookingFor == ItemType.Food).First().comAmountPerTick = (int)-(foodPerBillion * population);
        }
        else
        {
            PlanetProduction prod = new PlanetProduction(ItemType.Food, (int)-(foodPerBillion * population));
            dependencies.Add(prod);
            WarningLowResource(prod.typeLookingFor.ToString());
        }

        if (breathableAtmosphere) return;

        if (dependencies.Where(x => x.comProduced == null).Where(x => x.typeLookingFor == ItemType.Breathing).Count() != 0)
        {
            dependencies.Where(x => x.comProduced == null).Where(x => x.typeLookingFor == ItemType.Breathing).First().comAmountPerTick = (int)-(breathingPerBillion * population);
        }
        else
        {
            PlanetProduction prod = new PlanetProduction(ItemType.Breathing, (int)-(breathingPerBillion * population));
            dependencies.Add(prod);
            WarningLowResource(prod.typeLookingFor.ToString());
        }
    }

    public void WarningLowResource(string warning)
    {
        //Do a bunch of switch cases to do different messages for different warnings and resources lacking
    }

    public void TryForNewTradeRoutes(ItemType type, int amount)
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

    public void TryForNewTradeRoutes(BaseItem comm, int amount)
    {
        Debug.Log(planetName + " is trying to find routes for " + Mathf.Abs(amount) + " " + comm.itemName.ToString() + ".");

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

    public bool RequestTradeRoute(Planet asker, Planet giver, ItemType type, float amountRequested)
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

    public bool RequestTradeRoute(Planet asker, Planet giver, BaseItem comm, float amountRequested)
    {
        Debug.Log(asker.planetName + " asking for trade route with " + giver.planetName + " for " + Mathf.Abs(amountRequested) + " " + comm.itemName);
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

    public void SetUpTradeRoute(Planet asker, Planet giver, ItemType type, int amount)
    {
        if (asker == giver) return;

        Debug.Log(asker.planetName + " setting up trade route with " + giver.planetName + " for " + Mathf.Abs(amount) + " " + type.ToString());

        TradeRoute route = new TradeRoute(giver, asker, type, (int)Mathf.Abs(amount));
        tmManager.AddNewRoute(route);
    }

    public void SetUpTradeRoute(Planet asker, Planet giver, BaseItem comm, int amount)
    {
        if (asker == giver) return;

        Debug.Log(asker.planetName + " setting up trade route with " + giver.planetName + " for " + Mathf.Abs(amount) + " " + comm.itemName.ToString());

        BaseItem commo = Instantiate(comm);
        //commo.ChangeToComm(comm, amount);

        TradeRoute route = new TradeRoute(giver, asker, commo, (int)Mathf.Abs(amount));
        tmManager.AddNewRoute(route);
    }

    public void GenerateResource(PlanetProduction prod)
    {
        if (commoditiesInMarket.Where(x => x.itemName == prod.comProduced.itemName).ToList().Count != 0)
        {
            commoditiesInMarket.Where(x => x.itemName == prod.comProduced.itemName).First().itemStack += prod.comAmountPerTick;
        }
        else
        {
            commoditiesInMarket.Add(ScriptableObject.Instantiate(prod.comProduced));
            commoditiesInMarket.Where(x => x.itemName == prod.comProduced.itemName).First().itemStack += prod.comAmountPerTick;
        }
    }

    public void SubtractResource(PlanetProduction prod)
    {
        try
        {
            List<BaseItem> comms = commoditiesInMarket.Where(x => x.itemType == prod.typeLookingFor).ToList();
            comms[0].itemStack -= Mathf.Abs(prod.comAmountPerTick);
        }
        catch
        {
            totalWealth -= prod.comAmountPerTick * population;
        }           
    }

    public bool SubtractResource(BaseItem commToTake, int amount)
    {
        BaseItem comm = commoditiesInMarket.Where(x => x.itemName == commToTake.itemName).First();

        if(comm.itemStack >= Mathf.Abs(amount))
        {
            comm.itemStack -= Mathf.Abs(amount);
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ReceiveCommodity(BaseItem comm)
    {
        if (commoditiesInMarket.Where(x => x.itemName == comm.itemName).Count() != 0)
        {
            commoditiesInMarket.Where(x => x.itemName == comm.itemName).First().itemStack += comm.itemStack;
        }
        else
        {
            commoditiesInMarket.Add(comm);            
        }
    }  

    public List<Planet> FindPlanetsSupplyingCommodity(BaseItem comToSearch)
    {
        List<Planet> planets = allPlanets.Where(x => x.products.
        Where(x => x.comProduced.itemName == comToSearch.itemName).ToList().Count >= 0 && x.isTakenOverByAliens == false).ToList();

        return planets;
    }

    public List<Planet> FindPlanetsSupplyingCommodityType(ItemType type)
    {
        List<Planet> planets = allPlanets.Where(x => x.products.
        Where(x => x.comProduced.itemType == type).ToList().Count >= 0 && x.isTakenOverByAliens == false).ToList();

        return planets;
    }
}
