using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class BlinkingLights : MonoBehaviour
{
    public float minimum = 0.1f;
    public float maximum = 0.7f;

    private float cooldown;
    private float nextFire = 0;

    private Light2D lightt;

    private void Start()
    {
        lightt = GetComponent<Light2D>();
    }

    private void Update()
    {
        if (nextFire <= 0)
        {
            lightt.enabled = !lightt.enabled;
            nextFire = cooldown;
            cooldown = Random.Range(0.1f, 0.7f);
        }

        nextFire -= Time.deltaTime;
    }
}
