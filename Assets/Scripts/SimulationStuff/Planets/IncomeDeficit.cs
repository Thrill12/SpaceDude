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

        deficit += GetDeficitOfCommodityFromPlanetDependencies(planet, comm);
        deficit += GetDeficitOfCommodityFromPlanetTradeRoutes(planet, comm);

        profit = income - deficit;

        if(profit < 0)
        {
            Debug.Log(planet.planetName + " is suffering a loss of " + profit);
        }

        return profit;
    }

    public static float CalculateProfit(Planet planet, Commodity.Type type)
    {
        float profit = 0;

        float income = 0, deficit = 0;

        income += GetIncomeOfCommodityFromPlanetProducts(planet, type);
        income += GetIncomeOfCommodityFromPlanetTradeRoutes(planet, type);

        deficit += GetDeficitOfCommodityFromPlanetDependencies(planet, type);
        deficit += GetDeficitOfCommodityFromPlanetTradeRoutes(planet, type);

        profit = income - deficit;

        if (profit < 0)
        {
            Debug.Log(planet.planetName + " is suffering a loss of " + profit);
        }

        return profit;
    }

    private static float GetDeficitOfCommodityFromPlanetTradeRoutes(Planet planet, Commodity comm)
    {
        float inc = planet.availableTradeRoutes.Where(x => x.commodityToTransport.commodityName == comm.commodityName)
            .Where(x => x.sender == planet).Sum(x => x.commodityToTransport.stack);
        return inc;
    }

    private static float GetDeficitOfCommodityFromPlanetTradeRoutes(Planet planet, Commodity.Type type)
    {
        float inc = planet.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type)
                .Where(x => x.sender == planet).Sum(x => x.commodityToTransport.stack);
        return inc;
    }

    private static float GetDeficitOfCommodityFromPlanetDependencies(Planet planet, Commodity comm)
    {
        float def = planet.dependencies.Where(x => x.comProduced != null && x.comProduced.commodityName == comm.commodityName).Sum(x => x.comAmountPerTick);
        return def;      
    }

    private static float GetDeficitOfCommodityFromPlanetDependencies(Planet planet, Commodity.Type type)
    {
        Debug.Log("Type");
        float def = planet.dependencies.Where(x => x.lookingForTypeOnly == true).Where(x => x.typeLookingFor == type).Sum(x => x.comAmountPerTick);
        return def;
    }

    private static float GetIncomeOfCommodityFromPlanetTradeRoutes(Planet planet, Commodity comm)
    {
        float inc = planet.availableTradeRoutes.Where(x => x.commodityToTransport.commodityName == comm.commodityName)
            .Where(x => x.receiver == planet).Sum(x => x.commodityToTransport.stack);
        return inc;        
    }

    private static float GetIncomeOfCommodityFromPlanetTradeRoutes(Planet planet, Commodity.Type type)
    {
        float inc = planet.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type)
            .Where(x => x.receiver == planet).Sum(x => x.commodityToTransport.stack);
        return inc;
    }

    private static float GetIncomeOfCommodityFromPlanetProducts(Planet planet, Commodity comm)
    {
        float inc = planet.products.Where(x => x.comProduced != null && x.comProduced.commodityName == comm.commodityName).Sum(x => x.comAmountPerTick);
        return inc;             
    }

    private static float GetIncomeOfCommodityFromPlanetProducts(Planet planet, Commodity.Type type)
    {
        float inc = planet.products.Where(x => x.lookingForTypeOnly == true).Where(x => x.typeLookingFor == type).Sum(x => x.comAmountPerTick);
        return inc;
    }

    //public static float CalculateProfit(Planet planet, Commodity comm)
    //{
    //    planetToTrack = planet;
    //    commodityToTrack = comm;

    //    float profit = 0;

    //    float income = 0, deficit = 0;

    //    income += GetProfitForCommodityInProducts(income);
    //    deficit += GetDeficitForCommodityInDependencies(deficit);

    //    if (planetToTrack.availableTradeRoutes.Count > 0)
    //    {
    //        income += GetProfitForCommodityInTradeRoute(income);
    //        deficit += GetDeficitForCommodityInTradeRoute(deficit);
    //    }        

    //    profit = income + deficit;

    //    return profit;
    //}

    //private static float GetProfitForCommodityInProducts(float income)
    //{
    //    float newSum = 0;
    //    foreach (var item in planetToTrack.products.Where(x => x.comProduced.commodityName == commodityToTrack.commodityName))
    //    {
    //        newSum += item.comAmountPerTick;
    //    }

    //    return newSum;
    //}

    //private static float GetDeficitForCommodityInDependencies(float deficit)
    //{
    //    List<PlanetProduction> prods = planetToTrack.dependencies.Where(x => x.lookingForTypeOnly == false).ToList();
    //    Debug.Log(commodityToTrack.commodityName);
    //    List<PlanetProduction> prodsWithName = prods.Where(x => x.comProduced.commodityName == commodityToTrack.commodityName).ToList();

    //    if(prodsWithName.Count > 0)
    //    {
    //        float newSum = 0;
    //        foreach (var item in prodsWithName)
    //        {
    //            newSum += item.comAmountPerTick;
    //        }

    //        return newSum;
    //    }

    //    return 0;
    //}

    //private static float GetDeficitForCommodityInTradeRoute(float deficit)
    //{
    //    if (planetToTrack.availableTradeRoutes.Where(x => x.sender == planetToTrack).Count() == 0) return 0;

    //    float newSum = 0;
    //    foreach (var item in planetToTrack.availableTradeRoutes.Where(x => x.sender == planetToTrack).Where(x => x.commodityToTransport.commodityName == commodityToTrack.commodityName))
    //    {
    //        newSum += item.commodityToTransport.stack;
    //    }

    //    return newSum;
    //}

    //private static float GetProfitForCommodityInTradeRoute(float income)
    //{
    //    if (planetToTrack.availableTradeRoutes.Where(x => x.receiver == planetToTrack).Count() == 0) return 0;

    //    float newSum = 0;
    //    foreach (var item in planetToTrack.availableTradeRoutes.Where(x => x.receiver == planetToTrack).Where(x => x.commodityToTransport.commodityName == commodityToTrack.commodityName))
    //    {
    //        newSum += item.commodityToTransport.stack;
    //    }

    //    return newSum;
    //}

    //public static float CalculateProfit(Planet planet, Commodity.Type type)
    //{
    //    planetToTrack = planet;
    //    commodityTypeToTrack = type;

    //    float profit = 0;

    //    float income = 0, deficit = 0;

    //    if (planetToTrack.products.Where(x => x.typeLookingFor == type).Count() > 0)
    //    {
    //        income += planetToTrack.products.Where(x => x.typeLookingFor == type)
    //            .Sum(x => x.comAmountPerTick);
    //    }

    //    if (planetToTrack.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type).Where(x => x.receiver == planetToTrack).Count() > 0)
    //    {
    //        income += planetToTrack.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type)
    //            .Where(x => x.receiver == planetToTrack)
    //            .Sum(x => x.commodityToTransport.stack);
    //    }

    //    if (planetToTrack.dependencies.Where(x => x.typeLookingFor == type).Count() > 0)
    //    {
    //        deficit += planetToTrack.dependencies.Where(x => x.typeLookingFor == type)
    //            .Sum(x => x.comAmountPerTick);
    //    }

    //    if (planetToTrack.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type).Where(x => x.sender == planetToTrack).Count() > 0)
    //    {
    //        deficit += planetToTrack.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type)
    //            .Where(x => x.sender == planetToTrack)
    //            .Sum(x => x.commodityToTransport.stack);
    //    }

    //    profit = income + deficit;

    //    return profit;
    //}
}
