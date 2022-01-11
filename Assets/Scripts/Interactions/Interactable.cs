using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Interactable : MonoBehaviour
{
    private bool canInteract;

    public GameObject interactPromptObject;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && canInteract)
        {
            Interact();
        }
    }

    public virtual void Interact()
    {
        Debug.Log(gameObject.name + " interacted with");
    }

    public virtual void OnTriggerEnter2D(Collider2D collision)
    {   
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerSuit"))
        {
            canInteract = true;
            interactPromptObject.SetActive(true);
        }
    }

    public virtual void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("PlayerSuit"))
        {
            canInteract = false;
            interactPromptObject.SetActive(false);
        }
    }
}
