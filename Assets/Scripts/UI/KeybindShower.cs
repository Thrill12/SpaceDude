using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class KeybindShower : MonoBehaviour
{
    private PlayerInput playerInput;

    public GameObject text;
    public GameObject image;

    private void Start()
    {
        playerInput = UIManager.instance.playerInput;
    }

    private void Update()
    {
        if (playerInput.currentControlScheme == "GamePad")
        {
            image.SetActive(true);
            text.SetActive(false);
        }
        else
        {
            image.SetActive(false);
            text.SetActive(true);
        }
    }
}
