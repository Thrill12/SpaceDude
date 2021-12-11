using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ShootingScript : MonoBehaviour
{
    public GameObject projectile;
    public float speed;
    public float cooldown;
    public AudioClip shootClip;
    public GameObject shootSourcesParent;

    private AudioSource src;
    private float nextFire = 0;

    private List<GameObject> usedObjs = new List<GameObject>();
    private List<GameObject> children = new List<GameObject>();
    private PlayerShipMovement movement;
    private UIManager ui;

    private void Start()
    {
        src = GetComponent<AudioSource>();
        movement = transform.parent.GetComponent<PlayerShipMovement>();
        ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();

        foreach (Transform trans in shootSourcesParent.transform)
        {
            children.Add(trans.gameObject);
        }
    }

    private void Update()
    {
        if (movement.isPlayerInShip == false) return;
        if (ui.isInUI) return;

        if (nextFire <= 0)
        {
            if (Input.GetAxis("Fire1") != 0)
            {
                Shoot();
                nextFire = cooldown;
            }
        }

        nextFire -= Time.deltaTime;
    }

    public void Shoot()
    {
        var availableSources = children.Except(usedObjs).Union(usedObjs.Except(children));

        if(availableSources.Count() == 0)
        {
            availableSources = children;
        }

        if (usedObjs.SequenceEqual(children))
        {
            usedObjs = new List<GameObject>();
        }                                 

        GameObject spawn = availableSources.First();

        usedObjs.Add(spawn);

        GameObject proj = Instantiate(projectile, spawn.transform.position, Quaternion.identity);
        proj.GetComponent<Rigidbody2D>().velocity = -transform.up * (speed + transform.parent.GetComponent<Rigidbody2D>().velocity.magnitude);

        Vector2 v = proj.GetComponent<Rigidbody2D>().velocity;
        var angle = Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
        proj.transform.rotation = Quaternion.AngleAxis(angle - 90, Vector3.forward);

        src.PlayOneShot(shootClip);
    }
}
