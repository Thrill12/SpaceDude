using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlideyDoor : MonoBehaviour
{
    public Vector2 moveVector;
    public bool secure = false;
    public bool isOpen = false;
    public Transform doorObject;

    private Vector2 startPosition;

    private void Start()
    {
        startPosition = doorObject.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isOpen) return;

        if (!secure)
        {
            OpenDoor();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!isOpen) return;
        CloseDoor();
    }

    public void OpenDoor()
    {
        if (isOpen) return;
        LeanTween.move(doorObject.gameObject, startPosition + moveVector, 1);
        isOpen = true;
    }

    public void CloseDoor()
    {
        if (!isOpen) return;
        LeanTween.move(doorObject.gameObject, startPosition, 1);
        isOpen = false;
    }
}
