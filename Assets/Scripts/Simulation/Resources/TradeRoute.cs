using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TradeRoute
{
    public Planet sender;
    public Planet receiver;
    public Commodity commodityToTransport;
    public Commodity.Type commTypeToTransport = Commodity.Type.None;
    [ConditionalField("commTypeToTransport")]public float amount;

    public TradeRoute(Planet sender, Planet receiver, Commodity commToTransport, float amount)
    {
        this.sender = sender;
        this.receiver = receiver;
        commodityToTransport = new Commodity(commToTransport, amount);
    }

    public TradeRoute(Planet sender, Planet receiver, Commodity.Type commTypeToTransport, float amount)
    {
        this.sender = sender;
        this.receiver = receiver;
        this.commTypeToTransport = commTypeToTransport;
        this.amount = amount;
    }
}
