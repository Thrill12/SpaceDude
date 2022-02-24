using FullSerializer;
using System;
using UnityEngine;

public class Interactable : MonoBehaviour
{
    private bool canInteract;
    [fsIgnore]
    public GameObject interactPromptObject;

    public virtual void Interact()
    {
        
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {   
        if(collision.CompareTag("PlayerSuit"))
        {
            canInteract = true;
            interactPromptObject.SetActive(true);
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("PlayerSuit"))
        {
            canInteract = false;
            interactPromptObject.SetActive(false);
        }
    }
}
