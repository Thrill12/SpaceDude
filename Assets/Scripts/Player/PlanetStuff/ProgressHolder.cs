using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


//Holds progress done by the player, will probably use this as a place to call saving/loading functions, 
// and will probably act as storage for the databank on the ship
public class ProgressHolder : MonoBehaviour
{
    public static ProgressHolder instance;

    public List<Planet> planetsDiscovered;
    public List<InfluentialPerson> peopleDiscovered;

    public GameObject planetsDiscoveredLayout;
    public PrefabManager pfManager;

    private void Awake()
    {
        instance = this;
    }

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
