using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Globalization;

public class Station : MonoBehaviour
{
    public string stationName;
    public float difficultyValue;
    public UIManager ui;
    public TextAsset stationNames;
    public TextAsset stationPrefixes;

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

        stationName = nameChosen.Trim() + " " + prefixChosen.Trim();
    }

    private void Start()
    {
        GenerateName();
    }

    private void ToggleStationDisplay(bool toggleBool)
    {
        ui.stationNameDisplay.text = stationName;

        CanvasGroup canv = ui.mainStationDisplay.GetComponent<CanvasGroup>();

        if(toggleBool == true)
        {
            LeanTween.cancel(canv.gameObject);
            LeanTween.alphaCanvas(canv, 1, 1);
            LeanTween.alphaCanvas(canv, 0, 1).setDelay(5f);
        }
        else if(toggleBool == false)
        {
            LeanTween.cancel(canv.gameObject);
            LeanTween.alphaCanvas(canv, 0, 1).setDelay(1f);
        }            
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        ToggleStationDisplay(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        ToggleStationDisplay(false);
    }
}
