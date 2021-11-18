using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IncomeDeficit
{
    public static float CalculateProfit(Planet planet, Commodity comm)
    {
        float profit = 0;

        float income = 0, deficit = 0;

        income += GetIncomeOfCommodityFromPlanetProducts(planet, comm);
        income += GetIncomeOfCommodityFromPlanetTradeRoutes(planet, comm);

        deficit += Mathf.Abs(GetDeficitOfCommodityFromPlanetDependencies(planet, comm));
        deficit += Mathf.Abs(GetDeficitOfCommodityFromPlanetTradeRoutes(planet, comm));

        profit = income - Mathf.Abs(deficit);

        return profit;
    }

    public static float CalculateProfit(Planet planet, Commodity.Type type)
    {
        float profit = 0;

        float income = 0, deficit = 0;

        income += GetIncomeOfCommodityFromPlanetProducts(planet, type);
        income += GetIncomeOfCommodityFromPlanetTradeRoutes(planet, type);

        deficit += Mathf.Abs(GetDeficitOfCommodityFromPlanetDependencies(planet, type));
        deficit += Mathf.Abs(GetDeficitOfCommodityFromPlanetTradeRoutes(planet, type));

        profit = income - Mathf.Abs(deficit);

        return profit;
    }

    private static float GetDeficitOfCommodityFromPlanetTradeRoutes(Planet planet, Commodity comm)
    {
        float def = planet.availableTradeRoutes.Where(x => x.commodityToTransport != null && comm != null && x.commodityToTransport.commodityName == comm.commodityName)
            .Where(x => x.sender == planet).Sum(x => x.commodityToTransport.stack);
        return def;
    }

    private static float GetDeficitOfCommodityFromPlanetTradeRoutes(Planet planet, Commodity.Type type)
    {
        float def = planet.availableTradeRoutes.Where(x => x.commTypeToTransport == type)
                .Where(x => x.sender == planet).Sum(x => x.amount);
        return def;
    }

    private static float GetDeficitOfCommodityFromPlanetDependencies(Planet planet, Commodity comm)
    {
        float def = planet.dependencies.Where(x => comm != null && x.comProduced != null && x.comProduced.commodityName == comm.commodityName).Sum(x => x.comAmountPerTick);
        return def;      
    }

    private static float GetDeficitOfCommodityFromPlanetDependencies(Planet planet, Commodity.Type type)
    {
        float def = planet.dependencies.Where(x => x.lookingForTypeOnly == true && x.typeLookingFor == type).Sum(x => x.comAmountPerTick);
        return def;
    }

    private static float GetIncomeOfCommodityFromPlanetTradeRoutes(Planet planet, Commodity comm)
    {
        float inc = planet.availableTradeRoutes.Where(x => x.commodityToTransport != null && comm != null && x.receiver == planet && x.commodityToTransport.commodityName == comm.commodityName)
            .Sum(x => x.commodityToTransport.stack);
        return inc;        
    }

    private static float GetIncomeOfCommodityFromPlanetTradeRoutes(Planet planet, Commodity.Type type)
    {
        float inc = planet.availableTradeRoutes.Where(x => x.commTypeToTransport == type)
            .Where(x => x.receiver == planet).Sum(x => x.amount);
        return inc;
    }

    private static float GetIncomeOfCommodityFromPlanetProducts(Planet planet, Commodity comm)
    {
        float inc = planet.products.Where(x => x.lookingForTypeOnly == false && comm != null && x.comProduced.commodityName == comm.commodityName).Sum(x => x.comAmountPerTick);
        return inc;                
    }

    private static float GetIncomeOfCommodityFromPlanetProducts(Planet planet, Commodity.Type type)
    {
        float inc = planet.products.Where(x => x.comProduced.commodityType == type).Sum(x => x.comAmountPerTick);
        return inc;
    }
}
