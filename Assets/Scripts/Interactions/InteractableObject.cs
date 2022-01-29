using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class InteractableObject : Interactable
{
    public UnityEvent eventAction;

    public override void Interact()
    {
        base.Interact();
        eventAction.Invoke();
    }
}
