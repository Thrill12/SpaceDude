using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressHolderd : MonoBehaviour
{
    public List<Planet> planetsDiscovered;
    public List<InfluentialPerson> peopleDiscovered;

    public GameObject planetsDiscoveredLayout;
    public PrefabManager pfManager;

    public void AddPlanet(GameObject planet)
    {
        if (!planetsDiscovered.Contains(planet.GetComponent<Planet>()))
        {
            GameObject newUI = Instantiate(pfManager.planetHolderUI, planetsDiscoveredLayout.transform);
            newUI.transform.Find("PlanetSprite").GetComponent<Image>().sprite = planet.GetComponent<SpriteRenderer>().sprite;
            planetsDiscovered.Add(planet.GetComponent<Planet>());
        }      
    }
}