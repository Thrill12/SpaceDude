using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ProgressHex : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string hexName;
    [TextArea(5, 15)]
    public string hexDescription;
    public HexType hexType;
    public bool isDiscovered = false;

    private TMP_Text text;

    private ProgressHolder progress;
    private UIManager uiManager;

    private void Start()
    {
        text = GetComponentInChildren<TMP_Text>();
        progress = GameObject.FindGameObjectWithTag("Progress").GetComponent<ProgressHolder>();
        uiManager = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }

    public void Update()
    {
        if(hexType == HexType.Item)
        {
            if (progress.itemsDiscovered.Any(x => x.itemName == hexName))
            {
                text.text = hexName;
                isDiscovered = true;
            }
            else
            {
                text.text = "???";
            }
        }
        else if(hexType == HexType.Location)
        {
            if (progress.locationsDiscovered.Any(x => x.locationName == hexName))
            {
                text.text = hexName;
                isDiscovered = true;
            }
            else
            {
                text.text = "???";
            }
        }
        else if(hexType == HexType.NPC)
        {
            if (progress.npcsDiscovered.Any(x => x.npcName == hexName))
            {
                text.text = hexName;
                isDiscovered = true;
            }
            else
            {
                text.text = "???";
            }
        }

        if(EventSystem.current.currentSelectedGameObject == gameObject)
        {
            uiManager.DisplayDiscoverySelected(this);           
        }
    }

    public void OnPointerEnter(PointerEventData data)
    {
        uiManager.DisplayDiscoverySelected(this);

        uiManager.SelectUIObject(gameObject);
    }

    public void OnPointerExit(PointerEventData data)
    {
        uiManager.SelectUIObject(null);
    }
}

public enum HexType
{
    Item,
    Location,
    NPC,
    Quest
}
