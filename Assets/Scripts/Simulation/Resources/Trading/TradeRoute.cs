using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TradeRoute
{
    public Planet sender;
    public Planet receiver;
    public BaseItem itemToTransport;
    public ItemType itemTypeToTransport = ItemType.None;
    [ConditionalField("itemTypeToTransport")]public int amount;

    public TradeRoute(Planet sender, Planet receiver, BaseItem itToTransport, int amount)
    {
        this.sender = sender;
        this.receiver = receiver;
        if (typeof(GeneralItem).IsAssignableFrom(itToTransport.GetType()))
        {
            itemToTransport = itToTransport;
            GeneralItem genItem = (GeneralItem)itemToTransport;
            genItem.itemStack = amount;
            itemToTransport = genItem;
        }
        else
        {
            itemToTransport = itToTransport;
        }       
    }

    public TradeRoute(Planet sender, Planet receiver, ItemType itemTypeToTransport, int amount)
    {
        this.sender = sender;
        this.receiver = receiver;
        this.itemTypeToTransport = itemTypeToTransport;
        this.amount = amount;
    }
}
