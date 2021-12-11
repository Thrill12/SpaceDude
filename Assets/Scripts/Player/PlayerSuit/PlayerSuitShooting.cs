using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayerSuitShooting : MonoBehaviour
{
    public float projSpeed;
    public float shootCooldown;

    private PrefabManager pf;
    private GameObject projectile;
    private Vector2 mousePos;
    private float nextFire;

    private void Start()
    {
        pf = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        projectile = pf.playerSuitBullet;
        nextFire = 0;
    }

    private void Update()
    {
        if (nextFire <= 0)
        {
            if (Input.GetAxis("Fire1") != 0)
            {
                Shoot();
                nextFire = shootCooldown;
            }
        }

        nextFire -= Time.deltaTime;
    }

    private void Shoot()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        GameObject bullet = Instantiate(projectile, transform.position, Quaternion.identity);
        bullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
        bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right.normalized * (projSpeed + GetComponent<Rigidbody2D>().velocity.magnitude);
        Destroy(bullet, 3);

        ShakeCamera();
    }

    private void ShakeCamera()
    {
        GetComponent<CinemachineImpulseSource>().GenerateImpulse();
    }
}
