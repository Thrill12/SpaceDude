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

    public static float CalculateProfit(Planet planet, Commodity.Type type, Commodity comm = null)
    {
        planetToTrack = planet;
        commodityToTrack = comm;
        commodityTypeToTrack = type;

        float profit = 0;

        float income = 0, deficit = 0;

        if(commodityToTrack != null)
        {
            income = planetToTrack.products.Where(x => x.comProduced.commodityName == commodityToTrack.commodityName).Sum(x => x.comAmountPerTick) +
                planetToTrack.availableTradeRoutes.Where(x => x.receiver == planetToTrack).Sum(x => x.commodityToTransport.stack); ;
            deficit = planetToTrack.dependencies.Where(x => x.comProduced.commodityName == commodityToTrack.commodityName).Sum(x => x.comAmountPerTick) +
                planetToTrack.availableTradeRoutes.Where(x => x.sender == planetToTrack).Sum(x => x.commodityToTransport.stack); ;
        }
        else
        {
            income = planetToTrack.products.Where(x => x.comProduced.commodityType == commodityTypeToTrack).Sum(x => x.comAmountPerTick) +
                planetToTrack.availableTradeRoutes.Where(x => x.receiver == planetToTrack).
                Where(x => x.commodityToTransport.commodityType == commodityTypeToTrack).Sum(x => x.commodityToTransport.stack);
            deficit = planetToTrack.dependencies.Where(x => x.typeLookingFor == commodityTypeToTrack).Sum(x => x.comAmountPerTick) +
                planetToTrack.availableTradeRoutes.Where(x => x.sender == planetToTrack).
                Where(x => x.commodityToTransport.commodityType == commodityTypeToTrack).Sum(x => x.commodityToTransport.stack);
        }

        profit = income + deficit;

        return profit;
    }
}
