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
  
    public TMP_Text planetName;
    public TMP_Text planetDescription;
    public TMP_Text marketUnitSelectorText;
    public TMP_Text populationCounter;

    public GameObject planetMarketLayout;
    public GameObject planetUI;
    public GameObject marketBuyButton;
    public GameObject marketSellButton;

    public Image planetImage;
    public Slider marketUnitSelector;     

    [Space(5)]

    [Header("Menus")]

    public GameObject journalObject;
    public GameObject inventory;

    [HideInInspector]
    public BaseItem commSelectedInMarketWindow;
    [HideInInspector]
    public int commSelectedInMarketWindowUnits;

    private PlayerShipMovement playerMovement;
    private PrefabManager pfManager;
    private GameObject player;
    private PlanetCheckerRaycast planetCheck;
    [HideInInspector]
    public List<Planet> allPlanets = new List<Planet>();
    private Inventory inv;

    public bool isInUI;

    private void Start()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerShipMovement>();
        player = GameObject.FindGameObjectWithTag("Player");
        pfManager = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        planetCheck = player.GetComponent<PlanetCheckerRaycast>();
        inv = player.GetComponent<Inventory>();

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
        populationCounter.text = "Population: " + planetCheck.planetHovered.GetComponent<Planet>().population + " billion";

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

    public void MarketSelectCommodityInMarketWindow(BaseItem comm)
    {
        commSelectedInMarketWindow = comm;
        marketUnitSelector.maxValue = comm.itemStack;
        marketUnitSelector.value = comm.itemStack;
    }

    public void MarketBuyCommodity()
    {
        try
        {
            if(commSelectedInMarketWindow.itemStack >= commSelectedInMarketWindowUnits)
            {
                commSelectedInMarketWindow.itemStack -= commSelectedInMarketWindowUnits;
                BaseItem newCommToAdd = ScriptableObject.Instantiate(commSelectedInMarketWindow);
                newCommToAdd.itemStack = commSelectedInMarketWindowUnits;

                inv.AddItemToInventory(newCommToAdd);
                if(commSelectedInMarketWindow.itemStack == 0)
                {
                    planetCheck.planetHoveredP.commoditiesInMarket.Remove(commSelectedInMarketWindow);
                }
            }
            else if (commSelectedInMarketWindow.itemStack == commSelectedInMarketWindowUnits)
            {
                commSelectedInMarketWindow.itemStack -= commSelectedInMarketWindowUnits;
                BaseItem newCommToAdd = ScriptableObject.Instantiate(commSelectedInMarketWindow);
                newCommToAdd.itemStack = commSelectedInMarketWindowUnits;

                inv.AddItemToInventory(newCommToAdd);
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
            if (commSelectedInMarketWindow.itemStack > commSelectedInMarketWindowUnits)
            {
                commSelectedInMarketWindow.itemStack -= commSelectedInMarketWindowUnits;
                BaseItem newCommToAdd = ScriptableObject.Instantiate(commSelectedInMarketWindow);
                newCommToAdd.itemStack = commSelectedInMarketWindowUnits;

                planetCheck.planetHoveredP.ReceiveCommodity(newCommToAdd);
            }
            else if (commSelectedInMarketWindow.itemStack == commSelectedInMarketWindowUnits)
            {
                commSelectedInMarketWindow.itemStack -= commSelectedInMarketWindowUnits;
                BaseItem newCommToAdd = ScriptableObject.Instantiate(commSelectedInMarketWindow);
                newCommToAdd.itemStack = commSelectedInMarketWindowUnits;

                planetCheck.planetHoveredP.ReceiveCommodity(newCommToAdd);
                player.GetComponent<Inventory>().items.Remove(commSelectedInMarketWindow);
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

        foreach (BaseItem item in planetCheck.planetHoveredP.commoditiesInMarket)
        {
            GameObject obj = Instantiate(pfManager.commodityMarketDisplayObject, planetMarketLayout.transform);
            obj.GetComponent<CommodityInvHolder>().commodityHeld = item;
            try
            {
                List<List<BaseItem>> commodities = allPlanets.Select(x => x.commoditiesInMarket).ToList();
                List<BaseItem> accCommodity = new List<BaseItem>();
                foreach (List<BaseItem> list in commodities)
                {
                    foreach (BaseItem comm in list)
                    {
                        accCommodity.Add(comm);
                    }
                }

                List<BaseItem> matching = accCommodity.Where(x => x.itemName == item.itemName).ToList();
                matching.Remove(item);
                float average = (float)matching.Average(x => x.itemValue);

                obj.transform.Find("Commodity Average Price").GetComponent<TMP_Text>().text = "$" + average;
            }
            catch
            {
                obj.transform.Find("Commodity Average Price").GetComponent<TMP_Text>().text = "Only sold here";
            }

            obj.transform.Find("Commodity Icon").GetComponent<Image>().sprite = item.itemIcon;
            obj.transform.Find("Commodity Name").GetComponent<TMP_Text>().text = item.itemName;
            obj.transform.Find("Commodity Price").GetComponent<TMP_Text>().text = "$" + item.itemValue;
            obj.transform.Find("Commodity Units").GetComponent<TMP_Text>().text = item.itemStack + " units";
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

        foreach (BaseItem item in player.GetComponent<Inventory>().items)
        {
            GameObject obj = Instantiate(pfManager.commodityMarketDisplayObject, planetMarketLayout.transform);
            obj.GetComponent<CommodityInvHolder>().commodityHeld = item;
            try
            {
                List<List<BaseItem>> commodities = allPlanets.Select(x => x.commoditiesInMarket).ToList();
                List<BaseItem> accCommodity = new List<BaseItem>();
                foreach (List<BaseItem> list in commodities)
                {
                    foreach (BaseItem comm in list)
                    {
                        accCommodity.Add(comm);
                    }
                }

                List<BaseItem> matching = accCommodity.Where(x => x.itemName == item.itemName).ToList();
                matching.Remove(item);
                float average = (float)matching.Average(x => x.itemValue);

                obj.transform.Find("Commodity Average Price").GetComponent<TMP_Text>().text = "$" + average;
            }
            catch
            {
                obj.transform.Find("Commodity Average Price").GetComponent<TMP_Text>().text = "Only sold here";
            }

            obj.transform.Find("Commodity Icon").GetComponent<Image>().sprite = item.itemIcon;
            obj.transform.Find("Commodity Name").GetComponent<TMP_Text>().text = item.itemName;
            obj.transform.Find("Commodity Price").GetComponent<TMP_Text>().text = "$" + item.itemValue;
            obj.transform.Find("Commodity Units").GetComponent<TMP_Text>().text = item.itemStack + " units";
        }
    }
}
