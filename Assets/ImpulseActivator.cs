using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ImpulseActivator : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("Run", 1f);
    }

    void Run()
    {
        CinemachineImpulseListener il = GetComponent<CinemachineImpulseListener>();
        il.enabled = true;
    }
}
