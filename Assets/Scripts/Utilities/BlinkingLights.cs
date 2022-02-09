using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class BlinkingLights : MonoBehaviour
{
    public float minimum = 0.1f;
    public float maximum = 0.7f;

    private float cooldown;
    private float nextFire = 0;

    private Light2D lightt;
    private float startIntensity;

    //private void Awake()
    //{
    //    lightt = GetComponent<Light2D>();
    //    startIntensity = lightt.intensity;
    //}

    //private void Update()
    //{
    //    if (nextFire <= 0)
    //    {
    //        if (lightt == null) return;
    //        if(lightt.intensity == startIntensity)
    //        {
    //            lightt.intensity = 0;
    //        }
    //        else
    //        {
    //            lightt.intensity = startIntensity;
    //        }
    //        nextFire = cooldown;
    //        cooldown = Random.Range(0.1f, 0.7f);
    //    }

    //    nextFire -= Time.deltaTime;
    //}
}
