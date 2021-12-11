using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class GalacticEventHandler : MonoBehaviour
{
    public List<GalacticEvent> allEvents = new List<GalacticEvent>();
    public GameObject galacticEventHolder;
    public GameObject galacticEventsMainObject;

    private PrefabManager pf;
    private void Start()
    {
        pf = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
    }

    public void AddNewGalacticEvent(string description, Planet source)
    {
        GalacticEvent galEvent = new GalacticEvent(description, source);
        allEvents.Add(galEvent);
        DisplayGalacticEvent(galEvent);
    }

    public void DisplayGalacticEvent(GalacticEvent galEvent)
    {
        GameObject instantiated = Instantiate(pf.galacticEventPrefab, galacticEventHolder.transform);
        instantiated.transform.Find("PlanetImage").GetComponent<Image>().sprite = 
            GameObject.FindGameObjectsWithTag("Planet").ToList().Where(x => x.GetComponent<Planet>() == galEvent.eventSource)
            .First().gameObject.GetComponent<SpriteRenderer>().sprite;
        instantiated.transform.Find("GalacticEventDetails").GetComponent<TMP_Text>().text = galEvent.eventDescription;
        galacticEventsMainObject.GetComponent<Animator>().SetTrigger("NewEvent");
    }
}
