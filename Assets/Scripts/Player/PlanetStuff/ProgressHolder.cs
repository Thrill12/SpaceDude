using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


//Holds progress done by the player, will probably use this as a place to call saving/loading functions, 
// and will probably act as storage for the databank on the ship
public class ProgressHolder : MonoBehaviour
{
    public List<Location> locationsDiscovered;
    public List<NPC> npcsDiscovered;
    public List<BaseItem> itemsDiscovered;

    public GameObject planetsDiscoveredLayout;
    public PrefabManager pfManager;

    private void Start()
    {
        GameManager.instance.LoadDiscoveries();

        GameManager.instance.gameEvents.onNPCCommunicate += Discover;
        GameManager.instance.gameEvents.onLocationEntered += Discover;
        GameManager.instance.gameEvents.onItemPickedUp += Discover;
    }

    public void Discover(Location location)
    {
        if (locationsDiscovered.Any(x => x.locationName == location.locationName)) return;

        locationsDiscovered.Add(location);
        Debug.Log("Discovered " + location.locationName);
    }

    public void Discover(NPC npc)
    {
        if (npcsDiscovered.Any(x => x.npcName == npc.npcName)) return;

        npcsDiscovered.Add(npc);
        Debug.Log("Discovered " + npc.npcName);
    }

    public void Discover(BaseItem item)
    {
        if (itemsDiscovered.Any(x => x.itemName == item.itemName)) return;

        itemsDiscovered.Add(item);
        Debug.Log("Discovered " + item.itemName);
    }

    //public void AddPlanet(GameObject planet)
    //{
    //    if (!planetsDiscovered.Contains(planet.GetComponent<Planet>()))
    //    {
    //        GameObject newUI = Instantiate(pfManager.planetHolderUI, planetsDiscoveredLayout.transform);
    //        newUI.transform.Find("PlanetSprite").GetComponent<Image>().sprite = planet.GetComponent<SpriteRenderer>().sprite;
    //        planetsDiscovered.Add(planet.GetComponent<Planet>());
    //    }      
    //}
}
