using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MultiSOObserver : MonoBehaviour, IObserver
{   
    public List<SOObservation> observations;

    protected virtual void OnEnable()
    {
        Subscribe();
    }

    protected virtual void OnDisable()
    {
        Unsubscribe();
    }

    private void Subscribe()
    {
        foreach (var item in observations)
        {
            if (!item.subscribed)
            {
                item.observable.Subscribe(this);
                item.subscribed = true;
            }
        }
    }

    private void Unsubscribe()
    {
        foreach (var item in observations)
        {
            if (item.subscribed)
            {
                item.observable.Unsubscribe(this);
                item.subscribed = false;
            }
        }
    }

    public abstract void Notify();
}

[System.Serializable]
public class SOObservation
{
    public ObservableSO observable;
    [HideInInspector]
    public bool subscribed = false;
}
