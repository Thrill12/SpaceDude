using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;

public class UIManager : MonoBehaviour
{
    [Header("In-Game UI")]

    public TMP_Text speedIndicator;
    public Image dampenersImage;

    [Space(5)]

    [Header("Planet UI")]

    public GameObject planetUI;
    public TMP_Text planetName;
    public TMP_Text planetDescription;
    public GameObject planetMarketLayout;
    public Image planetImage;
    public Slider marketUnitSelector;
    public TMP_Text marketUnitSelectorText;
    public GameObject marketBuyButton;
    public GameObject marketSellButton;

    [Space(5)]

    [Header("Menus")]

    public GameObject journalObject;
    public GameObject inventory;

    [HideInInspector]
    public Commodity commSelectedInMarketWindow;
    [HideInInspector]
    public int commSelectedInMarketWindowUnits;

    private PlayerMovement playerMovement;
    private PrefabManager pfManager;
    private GameObject player;
    private PlanetCheckerRaycast planetCheck;
    [HideInInspector]
    public List<Planet> allPlanets = new List<Planet>();
    private PlayerInventory inv;

    public bool isInUI;

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        player = GameObject.FindGameObjectWithTag("Player");
        pfManager = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        planetCheck = player.GetComponent<PlanetCheckerRaycast>();
        inv = player.GetComponent<PlayerInventory>();

        foreach (var item in GameObject.FindGameObjectsWithTag("Planet"))
        {
            allPlanets.Add(item.GetComponent<Planet>());
        }
    }

    private void Update()
    {
        if(playerMovement.currentSpeed < playerMovement.maxSpeed)
        {
            speedIndicator.text = Mathf.RoundToInt(playerMovement.currentSpeed * 10) + " m/s";
        }
        else
        {
            speedIndicator.text = Mathf.RoundToInt(playerMovement.maxSpeed * 10) + " m/s";
        }

        if (playerMovement.dampeners)
        {
            dampenersImage.enabled = true;
        }
        else
        {
            dampenersImage.enabled = false;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            CloseAllUI();
        }

        if (Input.GetButtonDown("Journal"))
        {           
            Journal();
        }

        if (Input.GetButtonDown("Inventory"))
        {
            Inventory();
        }

        #region RandomCheckStuff

        if(commSelectedInMarketWindow == null)
        {
            marketUnitSelector.value = 0;
            marketUnitSelectorText.text = "";
        }

        #endregion
    }

    public void CloseAllUI()
    {
        if (planetUI.activeInHierarchy)
        {
            PlanetDescription();
        }

        if (journalObject.activeInHierarchy)
        {
            Journal();
        }

        if (inventory.activeInHierarchy)
        {
            Inventory();
        }

        isInUI = false;
    }

    public void PlanetDescription()
    {
        if(planetUI.activeInHierarchy == false)
        {
            CloseAllUI();
        }       

        planetUI.SetActive(!planetUI.activeInHierarchy);

        if (planetUI.activeInHierarchy)
        {
            isInUI = true;
        }
        else
        {
            isInUI = false;
        }

        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }        

        planetName.text = planetCheck.planetHoveredP.planetName;
        planetDescription.text = planetCheck.planetHoveredP.planetDescription;
        planetImage.sprite = planetCheck.planetHovered.GetComponent<SpriteRenderer>().sprite;

        MarketSwitchToMarketComms();
    }

    public void Journal()
    {
        if (journalObject.activeInHierarchy == false)
        {
            CloseAllUI();
        }

        journalObject.SetActive(!journalObject.activeInHierarchy);

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }

        if (journalObject.activeInHierarchy)
        {
            isInUI = true;
        }
        else
        {
            isInUI = false;
        }
    }

    public void Inventory()
    {
        if (inventory.activeInHierarchy == false)
        {
            CloseAllUI();
        }

        inventory.SetActive(!inventory.activeInHierarchy);

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }

        if (inventory.activeInHierarchy)
        {
            isInUI = true;
            commSelectedInMarketWindow = null;
            commSelectedInMarketWindowUnits = 0;
        }
        else
        {
            isInUI = false;
        }
    }

    public void MarketSelectCommodityInMarketWindow(Commodity comm)
    {
        commSelectedInMarketWindow = comm;
        marketUnitSelector.maxValue = comm.stack;
        marketUnitSelector.value = comm.stack;
    }

    public void MarketBuyCommodity()
    {
        try
        {
            if(commSelectedInMarketWindow.stack >= commSelectedInMarketWindowUnits)
            {
                commSelectedInMarketWindow.stack -= commSelectedInMarketWindowUnits;
                Commodity newCommToAdd = ScriptableObject.Instantiate(commSelectedInMarketWindow);
                newCommToAdd.stack = commSelectedInMarketWindowUnits;

                inv.AddToInventory(newCommToAdd);
                if(commSelectedInMarketWindow.stack == 0)
                {
                    planetCheck.planetHoveredP.commoditiesInMarket.Remove(commSelectedInMarketWindow);
                }
            }
            else if (commSelectedInMarketWindow.stack == commSelectedInMarketWindowUnits)
            {
                commSelectedInMarketWindow.stack -= commSelectedInMarketWindowUnits;
                Commodity newCommToAdd = ScriptableObject.Instantiate(commSelectedInMarketWindow);
                newCommToAdd.stack = commSelectedInMarketWindowUnits;

                inv.AddToInventory(newCommToAdd);
                planetCheck.planetHoveredP.commoditiesInMarket.Remove(commSelectedInMarketWindow);
            }
            else
            {
                Debug.Log("Couldn't afford");
            }            
        }
        catch
        {
            Debug.Log("Nothing selected");
        }

        MarketSwitchToMarketComms();
    }

    public void MarketSellCommodity()
    {
        try
        {
            if (commSelectedInMarketWindow.stack > commSelectedInMarketWindowUnits)
            {
                commSelectedInMarketWindow.stack -= commSelectedInMarketWindowUnits;
                Commodity newCommToAdd = ScriptableObject.Instantiate(commSelectedInMarketWindow);
                newCommToAdd.stack = commSelectedInMarketWindowUnits;

                planetCheck.planetHoveredP.ReceiveCommodity(newCommToAdd);
            }
            else if (commSelectedInMarketWindow.stack == commSelectedInMarketWindowUnits)
            {
                commSelectedInMarketWindow.stack -= commSelectedInMarketWindowUnits;
                Commodity newCommToAdd = ScriptableObject.Instantiate(commSelectedInMarketWindow);
                newCommToAdd.stack = commSelectedInMarketWindowUnits;

                planetCheck.planetHoveredP.ReceiveCommodity(newCommToAdd);
                player.GetComponent<PlayerInventory>().commsInInventory.Remove(commSelectedInMarketWindow);
            }
            else
            {
                Debug.Log("Couldn't afford");
            }
        }
        catch
        {
            Debug.Log("Nothing selected");
        }

        MarketSwitchToInventoryComms();
    }

    public void MarketUnitSelectorSliderValueChanged()
    {
        try
        {
            commSelectedInMarketWindowUnits = Mathf.RoundToInt(marketUnitSelector.value);
            marketUnitSelectorText.text = $"{commSelectedInMarketWindowUnits} units";
        }
        catch
        {
            Debug.Log("Nothing selected");
        }
    }

    public void MarketSwitchToMarketComms()
    {
        marketBuyButton.SetActive(true);
        marketSellButton.SetActive(false);

        try
        {
            foreach (Transform child in planetMarketLayout.transform)
            {
                Destroy(child.gameObject);
            }
        }
        catch
        {
            Debug.Log("No children to remove");
        }

        foreach (Commodity item in planetCheck.planetHoveredP.commoditiesInMarket)
        {
            GameObject obj = Instantiate(pfManager.commodityMarketDisplayObject, planetMarketLayout.transform);
            obj.GetComponent<CommodityInvHolder>().commodityHeld = item;
            try
            {
                List<List<Commodity>> commodities = allPlanets.Select(x => x.commoditiesInMarket).ToList();
                List<Commodity> accCommodity = new List<Commodity>();
                foreach (List<Commodity> list in commodities)
                {
                    foreach (Commodity comm in list)
                    {
                        accCommodity.Add(comm);
                    }
                }

                List<Commodity> matching = accCommodity.Where(x => x.commodityName == item.commodityName).ToList();
                matching.Remove(item);
                float average = (float)matching.Average(x => x.commodityPrice);

                obj.transform.Find("Commodity Average Price").GetComponent<TMP_Text>().text = "$" + average;
            }
            catch
            {
                obj.transform.Find("Commodity Average Price").GetComponent<TMP_Text>().text = "Only sold here";
            }

            obj.transform.Find("Commodity Icon").GetComponent<Image>().sprite = item.commodityIcon;
            obj.transform.Find("Commodity Name").GetComponent<TMP_Text>().text = item.commodityName;
            obj.transform.Find("Commodity Price").GetComponent<TMP_Text>().text = "$" + item.commodityPrice;
            obj.transform.Find("Commodity Units").GetComponent<TMP_Text>().text = item.stack + " units";
        }
    }

    public void MarketSwitchToInventoryComms()
    {
        marketBuyButton.SetActive(false);
        marketSellButton.SetActive(true);

        try
        {
            foreach (Transform child in planetMarketLayout.transform)
            {
                Destroy(child.gameObject);
            }
        }
        catch
        {
            Debug.Log("No children to remove");
        }

        foreach (Commodity item in player.GetComponent<PlayerInventory>().commsInInventory)
        {
            GameObject obj = Instantiate(pfManager.commodityMarketDisplayObject, planetMarketLayout.transform);
            obj.GetComponent<CommodityInvHolder>().commodityHeld = item;
            try
            {
                List<List<Commodity>> commodities = allPlanets.Select(x => x.commoditiesInMarket).ToList();
                List<Commodity> accCommodity = new List<Commodity>();
                foreach (List<Commodity> list in commodities)
                {
                    foreach (Commodity comm in list)
                    {
                        accCommodity.Add(comm);
                    }
                }

                List<Commodity> matching = accCommodity.Where(x => x.commodityName == item.commodityName).ToList();
                matching.Remove(item);
                float average = (float)matching.Average(x => x.commodityPrice);

                obj.transform.Find("Commodity Average Price").GetComponent<TMP_Text>().text = "$" + average;
            }
            catch
            {
                obj.transform.Find("Commodity Average Price").GetComponent<TMP_Text>().text = "Only sold here";
            }

            obj.transform.Find("Commodity Icon").GetComponent<Image>().sprite = item.commodityIcon;
            obj.transform.Find("Commodity Name").GetComponent<TMP_Text>().text = item.commodityName;
            obj.transform.Find("Commodity Price").GetComponent<TMP_Text>().text = "$" + item.commodityPrice;
            obj.transform.Find("Commodity Units").GetComponent<TMP_Text>().text = item.stack + " units";
        }
    }
}
