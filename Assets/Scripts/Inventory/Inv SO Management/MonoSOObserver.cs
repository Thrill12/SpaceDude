using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonoSOObserver : MonoBehaviour, IObserver
{
    public ObservableSO observable;
    private bool subscribed = false;

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
        if (!subscribed)
        {
            observable.Subscribe(this);
            subscribed = true;
        }
    }

    private void Unsubscribe()
    {
        if (subscribed)
        {
            observable.Unsubscribe(this);
            subscribed=false;
        }
    }

    public abstract void Notify();
}
