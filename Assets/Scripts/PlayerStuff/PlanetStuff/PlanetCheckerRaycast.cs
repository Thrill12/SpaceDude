using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlanetCheckerRaycast : MonoBehaviour
{
    public bool isOverPlanet;
    public LayerMask planetLayer;

    public GameObject planetHovered;
    public Planet planetHoveredP;

    private PlayerShipMovement shipMovement;
    private GameObject planetBorder;

    private SpriteRenderer planetBorderRend;

    private PrefabManager pfManager;
    private Vector3 defaultPlanetBorderScale;

    private UIManager ui;
    private ProgressHolderd progress;

    private void Start()
    {
        pfManager = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        progress = GameObject.FindGameObjectWithTag("Progress").GetComponent<ProgressHolderd>();
        shipMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShipMovement>();
        planetBorder = Instantiate(pfManager.planetBorder, transform.position, Quaternion.identity);

        planetBorderRend = planetBorder.GetComponent<SpriteRenderer>();
        planetBorder.SetActive(false);
        defaultPlanetBorderScale = planetBorder.transform.localScale;
    }

    private void Update()
    {
        if (!shipMovement.isPlayerInShip) return;
        if (isOverPlanet)
        {
            planetBorder.SetActive(true);
            planetHoveredP = planetHovered.GetComponent<Planet>();
            planetBorder.transform.SetParent(planetHovered.transform);
            planetBorder.transform.position = planetHovered.transform.position;
            planetBorder.transform.localScale = defaultPlanetBorderScale;

            if (Input.GetButtonDown("Interact"))
            {
                ui.PlanetDescription();
                progress.AddPlanet(planetHovered);
            }
            
        }
        else
        {
            planetBorder.transform.SetParent(null);
            planetBorder.SetActive(false);
        }
    }

    void FixedUpdate()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, Mathf.Infinity, planetLayer))
        {
            if (hit.transform.CompareTag("Planet"))
            {
                Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward) * hit.distance, Color.yellow);
                planetHovered = hit.transform.gameObject;
                isOverPlanet = true;
            }
            else
            {
                isOverPlanet = false;
            }
        }
        else
        {
            isOverPlanet = false;
        }
    }
}
