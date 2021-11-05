using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class IncomeDeficit
{
    public static Commodity commodityToTrack;
    public static Commodity.Type commodityTypeToTrack;
    public static float income;
    public static float deficit;
    public static Planet planetToTrack;

    public static float CalculateProfit(Planet planet, Commodity comm)
    {
        planetToTrack = planet;
        commodityToTrack = comm;

        float profit = 0;

        float income = 0, deficit = 0;

        if(planetToTrack.products.Count() > 0)
        {
            if (planetToTrack.products.Where(x => x.comProduced.commodityName == commodityToTrack.commodityName).Count() > 0)
            {
                income += planetToTrack.products.Where(x => x.comProduced.commodityName == commodityToTrack.commodityName).Sum(x => x.comAmountPerTick);
            }
        }
            
        if(planetToTrack.availableTradeRoutes.Count() > 0)
        {
            if (planetToTrack.availableTradeRoutes.Where(x => x.receiver = planetToTrack).Where(x => x.commodityToTransport.commodityName == commodityToTrack.commodityName).Count() > 0)
            {
                income += planetToTrack.availableTradeRoutes.Where(x => x.receiver = planetToTrack).
                    Where(x => x.commodityToTransport.commodityName == commodityToTrack.commodityName).Sum(x => x.commodityToTransport.stack);
            }

            if (planetToTrack.availableTradeRoutes.Where(x => x.sender = planetToTrack).Where(x => x.commodityToTransport.commodityName == commodityToTrack.commodityName).Count() > 0)
            {
                deficit += planetToTrack.availableTradeRoutes.Where(x => x.sender = planetToTrack).
                    Where(x => x.commodityToTransport.commodityName == commodityToTrack.commodityName).Sum(x => x.commodityToTransport.stack);
            }
        }

        if(planetToTrack.dependencies.Count() > 0)
        {
            if (planetToTrack.dependencies.Where(x => x.lookingForTypeOnly == false).Where(x => x.comProduced.commodityName == commodityToTrack.commodityName).Count() > 0)
            {
                deficit += planetToTrack.dependencies.Where(x => x.comProduced.commodityName == commodityToTrack.commodityName).Sum(x => x.comAmountPerTick);
            }
        }                       

        profit = income + deficit; 

        return profit;
    }

    public static float CalculateProfit(Planet planet, Commodity.Type type)
    {
        planetToTrack = planet;
        commodityTypeToTrack = type;

        float profit = 0;

        float income = 0, deficit = 0;


        if (planetToTrack.products.Where(x => x.typeLookingFor == type).Count() > 0)
        {
            income += planetToTrack.products.Where(x => x.typeLookingFor == type).Sum(x => x.comAmountPerTick);
        }

        if (planetToTrack.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type).Where(x => x.receiver == planetToTrack).Count() > 0)
        {
            income += planetToTrack.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type).Where(x => x.receiver == planetToTrack).Sum(x => x.commodityToTransport.stack);
        }

        if (planetToTrack.dependencies.Where(x => x.typeLookingFor == type).Count() > 0)
        {
            deficit += planetToTrack.dependencies.Where(x => x.typeLookingFor == type).Sum(x => x.comAmountPerTick);
        }

        if (planetToTrack.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type).Where(x => x.sender == planetToTrack).Count() > 0)
        {
            deficit += planetToTrack.availableTradeRoutes.Where(x => x.commodityToTransport.commodityType == type).Where(x => x.sender == planetToTrack).Sum(x => x.commodityToTransport.stack);
        }


        profit = income + deficit;

        return profit;
    }
}
