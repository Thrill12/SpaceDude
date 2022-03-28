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
    [ConditionalField(nameof(shouldGeneateRandomly))] public LocationType locType = LocationType.Station;
    [ConditionalField(nameof(locType), false, LocationType.Station)]public TextAsset stationNames;
    [ConditionalField(nameof(locType), false, LocationType.Station)] public TextAsset stationPrefixes;

    public void GenerateNameWithPersonNamesAndPrefixes()
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

    public void GenerateNamesBasedOffPosition()
    {
        string fullName = "";

        string alphabet = "abcdefghijklmnopqrstuvwxyz";

        string firstHalf = alphabet[Random.Range(0, alphabet.Length)] + Mathf.RoundToInt(transform.position.x).ToString();
        string secondHalf = alphabet[Random.Range(0, alphabet.Length)] + Mathf.RoundToInt(transform.position.y).ToString();

        fullName = firstHalf.ToUpper() + "-" + secondHalf.ToUpper();

        locationName = fullName;
    }

    private void Start()
    {
        ui = GameManager.instance.uiManager;
        if (shouldGeneateRandomly)
        {
            if(locType == LocationType.Station)
            {
                GenerateNameWithPersonNamesAndPrefixes();
            }
            else if(locType == LocationType.AsteroidField)
            {
                GenerateNamesBasedOffPosition();
            }          
        }       
    }    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerSuit"))
        {
            ui.ToggleLocationNameDisplay(true, locationName);
            GameManager.instance.gameEvents.OnLocationEntered(this);
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("PlayerSuit"))
        {
            GameManager.instance.uiManager.ToggleLocationNameDisplay(false, locationName);
        }       
    }
}

public enum LocationType
{
    Station,
    AsteroidField
}
