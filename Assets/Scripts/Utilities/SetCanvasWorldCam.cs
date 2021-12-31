using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCanvasWorldCam : MonoBehaviour
{
    private void Start()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
    }
}
