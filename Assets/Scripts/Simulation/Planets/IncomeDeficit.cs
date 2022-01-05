using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IncomeDeficit
{
    //I think these pretty much do the same thing, but on different lists. Deficit is loss of item, profit is profit above loss.
    // We need this to calculate trade routes and stuff.

    public static int CalculateProfit(Planet planet, BaseItem comm)
    {
        int profit = 0;

        int income = 0, deficit = 0;

        income += GetIncomeOfCommodityFromPlanetProducts(planet, comm);
        income += GetIncomeOfCommodityFromPlanetTradeRoutes(planet, comm);

        deficit += Mathf.Abs(GetDeficitOfCommodityFromPlanetDependencies(planet, comm));
        deficit += Mathf.Abs(GetDeficitOfCommodityFromPlanetTradeRoutes(planet, comm));

        profit = income - Mathf.Abs(deficit);

        return profit;
    }

    public static int CalculateProfit(Planet planet, ItemType type)
    {
        int profit = 0;

        int income = 0, deficit = 0;

        income += GetIncomeOfCommodityFromPlanetProducts(planet, type);
        income += GetIncomeOfCommodityFromPlanetTradeRoutes(planet, type);

        deficit += Mathf.Abs(GetDeficitOfCommodityFromPlanetDependencies(planet, type));
        deficit += Mathf.Abs(GetDeficitOfCommodityFromPlanetTradeRoutes(planet, type));

        profit = income - Mathf.Abs(deficit);

        return profit;
    }

    private static int GetDeficitOfCommodityFromPlanetTradeRoutes(Planet planet, BaseItem comm)
    {
        int def = planet.availableTradeRoutes.Where(x => x.itemToTransport != null && comm != null && x.itemToTransport.itemName == comm.itemName)
        .Where(x => x.sender == planet).Sum(x => x.itemToTransport.itemStack);
        return def;
    }

    private static int GetDeficitOfCommodityFromPlanetTradeRoutes(Planet planet, ItemType type)
    {
        int def = planet.availableTradeRoutes.Where(x => x.itemTypeToTransport == type)
                .Where(x => x.sender == planet).Sum(x => x.amount);
        return def;
    }

    private static int GetDeficitOfCommodityFromPlanetDependencies(Planet planet, BaseItem comm)
    {
        int def = planet.dependencies.Where(x => comm != null && x.comProduced != null && x.comProduced.itemName == comm.itemName).Sum(x => x.comAmountPerTick);
        return def;      
    }

    private static int GetDeficitOfCommodityFromPlanetDependencies(Planet planet, ItemType type)
    {
        int def = planet.dependencies.Where(x => x.lookingForTypeOnly == true && x.typeLookingFor == type).Sum(x => x.comAmountPerTick);
        return def;
    }

    private static int GetIncomeOfCommodityFromPlanetTradeRoutes(Planet planet, BaseItem comm)
    {
        int inc = planet.availableTradeRoutes.Where(x => x.itemToTransport != null && comm != null && x.receiver == planet && x.itemToTransport.itemName == comm.itemName)
            .Sum(x => x.itemToTransport.itemStack);
        return inc;
    }

    private static int GetIncomeOfCommodityFromPlanetTradeRoutes(Planet planet, ItemType type)
    {
        int inc = planet.availableTradeRoutes.Where(x => x.itemTypeToTransport == type)
            .Where(x => x.receiver == planet).Sum(x => x.amount);
        return inc;
    }

    private static int GetIncomeOfCommodityFromPlanetProducts(Planet planet, BaseItem comm)
    {
        int inc = planet.products.Where(x => x.lookingForTypeOnly == false && comm != null && x.comProduced.itemName == comm.itemName).Sum(x => x.comAmountPerTick);
        return inc;                
    }

    private static int GetIncomeOfCommodityFromPlanetProducts(Planet planet, ItemType type)
    {
        int inc = planet.products.Where(x => x.comProduced.itemType == type).Sum(x => x.comAmountPerTick);
        return inc;
    }
}
