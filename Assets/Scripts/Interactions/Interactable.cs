using UnityEngine;

public class Interactable : MonoBehaviour
{
    private bool canInteract;

    public GameObject interactPromptObject;

    public virtual void Interact()
    {
        Debug.Log(gameObject.name + " interacted with");      
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
