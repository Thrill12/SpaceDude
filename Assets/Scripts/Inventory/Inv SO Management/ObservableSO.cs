using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IObserver
{
    void Notify();
}

public class ObservableSO : ScriptableObject
{
    List<IObserver> subscribers = new List<IObserver>();

    public void Subscribe(IObserver obs)
    {
        subscribers.Add(obs);
    }

    public void Unsubscribe(IObserver obs)
    {
        subscribers.Remove(obs);
    }

    public void Notify()
    {
        foreach (IObserver subscriber in subscribers)
        {
            subscriber.Notify();
        }
    }
}
