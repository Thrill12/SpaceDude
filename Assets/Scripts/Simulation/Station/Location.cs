using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Globalization;
using TMPro;
using MyBox;

public class Location : MonoBehaviour
{
    public string locationName;
    public float difficultyValue;
    private UIManager ui;
    public bool shouldGeneateRandomly = true;
    [ConditionalField("shouldGenerateRandomly")]public TextAsset stationNames;
    [ConditionalField("shouldGenerateRandomly")] public TextAsset stationPrefixes;

    public void GenerateName()
    {       
        string allNames = stationNames.text;
        List<string> splitNames = allNames.Split("\n").ToList();
        string nameChosen = splitNames[Random.Range(0, splitNames.Count)];

        string allprefixes = stationPrefixes.text;
        List<string> splitPrefixes = allprefixes.Split("\n").ToList();
        string prefixChosen = splitPrefixes[Random.Range(0, splitPrefixes.Count)];

        TextInfo txtInfo = new CultureInfo("en-US", true).TextInfo;

        nameChosen = txtInfo.ToTitleCase(nameChosen.ToLower());

        locationName = nameChosen.Trim() + " " + prefixChosen.Trim();
    }

    private void Start()
    {
        ui = UIManager.instance;
        if (shouldGeneateRandomly)
        {
            GenerateName();
        }       
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerSuit"))
        {
            UIManager.instance.ToggleLocationNameDisplay(true, locationName);
            GameEvents.instance.OnLocationEntered(this);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerSuit"))
        {
            UIManager.instance.ToggleLocationNameDisplay(false, locationName);
        }       
    }
}
