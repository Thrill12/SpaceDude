using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TradeRoute
{
    public readonly Planet sender;
    public readonly Planet receiver;
    public Commodity commodityToTransport;

    public TradeRoute(Planet sender, Planet receiver, Commodity commToTransport, float amount)
    {
        this.sender = sender;
        this.receiver = receiver;
        commodityToTransport = new Commodity(commToTransport, amount);
    }
}
