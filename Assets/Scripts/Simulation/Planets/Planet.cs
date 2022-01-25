using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [HideInInspector]
    public string planetName;
    public string planetDescription;

    [Space(10)]

    public bool breathableAtmosphere;
    public bool isTakenOverByAliens;

    [Space(10)]

    public PlanetClimate planetClimate;

    [Space(10)]

    public float totalInfluence;
    //In Billion
    public long population;
    public float totalWealth;
    public float influenceModifier;

    [Space(10)]

    [Header("Raw Resources Available")]

    public List<RawResource> resources;

    [Space(10)]

    public List<BaseItem> startingComms;

    [Space(10)]

    public List<BaseItem> commoditiesInMarket;
    public List<TradeRoute> availableTradeRoutes;

    [Space(10)]

    public List<PlanetProduction> products;
    public List<PlanetProduction> dependencies;

    private List<Planet> allPlanets;

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

    private void Start()
    {
        for (int i = 0; i < Random.Range(0, 4); i++)
        {
            AddRandomResource();
        }
    }

    private void AddRandomResource()
    {
        RawResource resourceChosen = ScriptableObject.Instantiate(RawResourceTable.instance.GetRandomResources());

        int randomStackChosen = Random.Range(50, 250);

        resourceChosen.itemStack = randomStackChosen;

        if (planetClimate.CanResourceGrow(resourceChosen))
        {
            if(resources.Any(x => x.itemName == resourceChosen.itemName))
            {
                resources.Where(x => x.itemName == resourceChosen.itemName).First().itemStack += randomStackChosen;
            }
            else
            {
                resources.Add(resourceChosen);
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
        
        if(Random.Range(1, 101) <= population)
        {
            AddRandomResource();
        }
    }

    //This will find all the deficits of the planet so that it has enough for its population/needs
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

    //This will be where the main population related stats will be calculated. Will need to
    // Split this up in multiple functions for different things, but it's okay for now.
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

    //Looking for trade routes throughout all known planets, will need to modify when we get to including factions in the game
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

    //Once the player has found a potential trade route, it needs to request it to the planet. Income Deficit is used to 
    // Calculate stats for the other planet as well, and will return true or false for a few reasons.
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

    //Once the player has found a potential trade route, it needs to request it to the planet. Income Deficit is used to 
    // Calculate stats for the other planet as well, and will return true or false for a few reasons.
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

    //Once trade route is accepted, this function runs to set up and start the trade route.
    // Currently, planets have no way of cancelling trade routes, so will need to do this too.
    public void SetUpTradeRoute(Planet asker, Planet giver, ItemType type, int amount)
    {
        if (asker == giver) return;

        Debug.Log(asker.planetName + " setting up trade route with " + giver.planetName + " for " + Mathf.Abs(amount) + " " + type.ToString());

        TradeRoute route = new TradeRoute(giver, asker, type, (int)Mathf.Abs(amount));
        tmManager.AddNewRoute(route);
    }

    //Once trade route is accepted, this function runs to set up and start the trade route.
    // Currently, planets have no way of cancelling trade routes, so will need to do this too.
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

    //Used byu cargo ships to give the planets the commodities they are carrying.
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
