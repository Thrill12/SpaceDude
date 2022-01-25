using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;

    public GameObject firstSelectedInPauseMenu;
    public PlayerInput playerInput;
    public AudioListener audioListener;

    [Header("In-Game UI")]

    public TMP_Text speedIndicator;

    public Image dampenersImage;

    [HideInInspector]
    public bool triggeredNextStep;
    public GameObject dialogueDisplay;
    public GameObject dialogueLocationEnabled;
    public GameObject dialogueLocationDisabled;
    public GameObject activeMissionsLog;
    private Vector3 originalDialogueDisplayPosition;

    [Space(5)]

    [Header("Station Display")]

    public TMP_Text stationNameDisplay;
    public TMP_Text difficultyDisplay;

    public GameObject mainStationDisplay;

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
    public GameObject pauseMenu;

    [Header("Inventory")]

    public ItemStatDisplayer leftItemStatDisplay;
    public ItemStatDisplayer rightItemStatDisplay;
    public GameObject invisSelectButton;
    public BaseItem currentlySelectedItemToDisplay;

    [Space(15)]

    [HideInInspector]
    public BaseItem commSelectedInMarketWindow;
    [HideInInspector]
    public int commSelectedInMarketWindowUnits;

    public PlayerShipMovement playerMovement;
    private PrefabManager pfManager;
    public GameObject player;
    private PlanetCheckerRaycast planetCheck;
    [HideInInspector]
    public List<Planet> allPlanets = new List<Planet>();
    public Inventory inv;

    public bool isInUI;

    [Space(5)]

    public AudioSource audioSource;
    public AudioClip typingClip;
    public float letterTypingPause;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        pfManager = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        planetCheck = player.GetComponent<PlanetCheckerRaycast>();

        foreach (var item in GameObject.FindGameObjectsWithTag("Planet"))
        {
            allPlanets.Add(item.GetComponent<Planet>());
        }

        audioSource = GetComponent<AudioSource>();
        originalDialogueDisplayPosition = dialogueDisplay.transform.position;

        playerInput.SwitchCurrentActionMap("PlayerShip");
    }

    private void Update()
    {
        Debug.Log(playerInput.currentActionMap.name);

        //Displaying the currently hovered on item in the inventory
        if (playerInput.currentControlScheme == "GamePad")
        {
            //Displaying the current item selected in the inventory
            if (currentlySelectedItemToDisplay != null && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<UIItemHolder>())
            {
                if (currentlySelectedItemToDisplay as BaseEquippable)
                {
                    BaseEquippable equip = (BaseEquippable)currentlySelectedItemToDisplay;

                    if (equip.isEquipped)
                    {
                        leftItemStatDisplay.gameObject.SetActive(true);
                        rightItemStatDisplay.gameObject.SetActive(false);
                        leftItemStatDisplay.GetComponent<ItemStatDisplayer>().ShowItem(currentlySelectedItemToDisplay);
                    }
                    else
                    {
                        leftItemStatDisplay.gameObject.SetActive(false);
                        rightItemStatDisplay.gameObject.SetActive(true);
                        rightItemStatDisplay.GetComponent<ItemStatDisplayer>().ShowItem(currentlySelectedItemToDisplay);
                    }
                }
                else
                {
                    leftItemStatDisplay.gameObject.SetActive(false);
                    rightItemStatDisplay.gameObject.SetActive(true);
                    rightItemStatDisplay.GetComponent<ItemStatDisplayer>().ShowItem(currentlySelectedItemToDisplay);
                }
            }
            else
            {
                leftItemStatDisplay.gameObject.SetActive(false);
                rightItemStatDisplay.gameObject.SetActive(false);
                currentlySelectedItemToDisplay = null;
            }
        }
        else
        {
            //Displaying the current item selected in the inventory
            if (currentlySelectedItemToDisplay != null)
            {
                if (currentlySelectedItemToDisplay as BaseEquippable)
                {
                    BaseEquippable equip = (BaseEquippable)currentlySelectedItemToDisplay;

                    if (equip.isEquipped)
                    {
                        leftItemStatDisplay.gameObject.SetActive(true);
                        rightItemStatDisplay.gameObject.SetActive(false);
                        leftItemStatDisplay.GetComponent<ItemStatDisplayer>().ShowItem(currentlySelectedItemToDisplay);
                    }
                    else
                    {
                        leftItemStatDisplay.gameObject.SetActive(false);
                        rightItemStatDisplay.gameObject.SetActive(true);
                        rightItemStatDisplay.GetComponent<ItemStatDisplayer>().ShowItem(currentlySelectedItemToDisplay);
                    }
                }
                else
                {
                    leftItemStatDisplay.gameObject.SetActive(false);
                    rightItemStatDisplay.gameObject.SetActive(true);
                    rightItemStatDisplay.GetComponent<ItemStatDisplayer>().ShowItem(currentlySelectedItemToDisplay);
                }
            }
            else
            {
                leftItemStatDisplay.gameObject.SetActive(false);
                rightItemStatDisplay.gameObject.SetActive(false);
                currentlySelectedItemToDisplay = null;
            }
        }

        if(playerInput.currentControlScheme == "GamePad")
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        if(playerMovement.inWarp == false)
        {
            if (playerMovement.currentSpeed < playerMovement.maxSpeed)
            {
                speedIndicator.text = Mathf.RoundToInt(playerMovement.currentSpeed * 10) + " m/s";
            }
            else
            {
                speedIndicator.text = Mathf.RoundToInt(playerMovement.maxSpeed * 10) + " m/s";
            }
        }
        else
        {
            if(Random.Range(0, 2) == 0)
            {
                speedIndicator.text = int.Parse(Random.Range(1000, 10000000).ToString("X"), System.Globalization.NumberStyles.HexNumber) + " m/s";
            }
            else
            {
                speedIndicator.text = Random.Range(1000, 10000000)+ " m/s";
            }           
        }       

        if (playerMovement.dampeners)
        {
            dampenersImage.enabled = true;
        }
        else
        {
            dampenersImage.enabled = false;
        }

        #region RandomCheckStuff

        if(commSelectedInMarketWindow == null)
        {
            marketUnitSelector.value = 0;
            marketUnitSelectorText.text = "";
        }

        #endregion
    }   

    #region NormalUI

    //This selects the first item in the inventory when the player switches to a gamepad midgame
    public void OnControlsChanged()
    {
        if (inventory.activeInHierarchy)
        {
            if (playerInput.currentControlScheme == "GamePad")
            {
                SelectFirstItemHolder();
            }
        }
    }

    //Makes sure all the active UI is closed
    public void CloseAllUI(InputAction.CallbackContext context)
    {
        if (planetUI.activeInHierarchy)
        {
            PlanetDescription(context);
        }

        if (journalObject.activeInHierarchy)
        {
            Journal(context);
        }

        if (inventory.activeInHierarchy)
        {
            Inventory(context);
        }

        if (pauseMenu.activeInHierarchy)
        {
            PauseMenu(context);
        }

        isInUI = false;

        if (playerMovement.isPlayerPiloting)
        {
            playerInput.SwitchCurrentActionMap("PlayerShip");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("PlayerSuit");
        }
    }

    //Turns on the planet interaction menu when the player is in the ship
    public void PlanetDescription(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if(planetCheck.planetHovered != null)
        {      
            ProgressHolder.instance.AddPlanet(planetCheck.planetHovered);

            if (planetUI.activeInHierarchy == false)
            {
                CloseAllUI(context);
            }

            if (playerInput.currentActionMap.name == "UI")
            {
                if (playerMovement.isPlayerPiloting)
                {
                    playerInput.SwitchCurrentActionMap("PlayerShip");
                }
                else
                {
                    playerInput.SwitchCurrentActionMap("PlayerSuit");
                }
            }
            else
            {
                playerInput.SwitchCurrentActionMap("UI");
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

            if (Time.timeScale == 0)
            {
                
                Time.timeScale = 1;
                StopAllCoroutines();
            }
            else
            {
                Time.timeScale = 0;
                TypePlanetDescription(planetCheck.planetHoveredP, planetDescription);
            }

            planetName.text = planetCheck.planetHoveredP.planetName;
            planetImage.sprite = planetCheck.planetHovered.GetComponent<SpriteRenderer>().sprite;
            populationCounter.text = "Population: " + planetCheck.planetHovered.GetComponent<Planet>().population + " billion";

            MarketSwitchToMarketComms();
        }      
    }

    //Turns on the journal menu for the player to keep track of their progress
    public void Journal(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;        

        if (journalObject.activeInHierarchy == false)
        {
            CloseAllUI(context);
        }

        if (playerInput.currentActionMap.name == "UI")
        {
            if (playerMovement.isPlayerPiloting)
            {
                playerInput.SwitchCurrentActionMap("PlayerShip");
            }
            else
            {
                playerInput.SwitchCurrentActionMap("PlayerSuit");
            }           
        }
        else
        {
            playerInput.SwitchCurrentActionMap("UI");
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

    //Turns on player inventory when the player is outside the ship
    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if (inventory.activeInHierarchy == false)
        {
            CloseAllUI(context);
        }

        if (playerInput.currentActionMap.name == "UI")
        {
            if (playerMovement.isPlayerPiloting)
            {
                playerInput.SwitchCurrentActionMap("PlayerShip");
            }
            else
            {
                playerInput.SwitchCurrentActionMap("PlayerSuit");
            }
        }
        else
        {
            playerInput.SwitchCurrentActionMap("UI");
        }

        if(playerInput.currentControlScheme == "GamePad")
        {
            Debug.Log("Selected");
            SelectFirstItemHolder();
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

    //Turns on pause menu and pauses the game
    public void PauseMenu(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        if (pauseMenu.activeInHierarchy == false)
        {
            CloseAllUI(context);
        }

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedInPauseMenu);


        if (playerInput.currentActionMap.name == "UI")
        {
            if (playerMovement.isPlayerPiloting)
            {
                playerInput.SwitchCurrentActionMap("PlayerShip");
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }
            else
            {
                playerInput.SwitchCurrentActionMap("PlayerSuit");
                pauseMenu.SetActive(false);
                Time.timeScale = 1;
            }


        }
        else
        {
            playerInput.SwitchCurrentActionMap("UI");
            pauseMenu.SetActive(true);
            Time.timeScale = 0;
        }

        if (pauseMenu.activeSelf)
        {
            isInUI = true;
        }
        else
        {
            isInUI = false;
        }
    }

    //Overloads for no context
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

        if (playerMovement.isPlayerPiloting)
        {
            playerInput.SwitchCurrentActionMap("PlayerShip");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("PlayerSuit");
        }
    }

    public void PlanetDescription()
    {
        if (planetCheck.planetHovered != null)
        {
            ProgressHolder.instance.AddPlanet(planetCheck.planetHovered);

            if (planetUI.activeInHierarchy == false)
            {
                CloseAllUI();
            }

            if (playerInput.currentActionMap.name == "UI")
            {
                if (playerMovement.isPlayerPiloting)
                {
                    playerInput.SwitchCurrentActionMap("PlayerShip");
                }
                else
                {
                    playerInput.SwitchCurrentActionMap("PlayerSuit");
                }
            }
            else
            {
                playerInput.SwitchCurrentActionMap("UI");
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

            if (Time.timeScale == 0)
            {

                Time.timeScale = 1;
                StopAllCoroutines();
            }
            else
            {
                Time.timeScale = 0;
                TypePlanetDescription(planetCheck.planetHoveredP, planetDescription);
            }

            planetName.text = planetCheck.planetHoveredP.planetName;
            planetImage.sprite = planetCheck.planetHovered.GetComponent<SpriteRenderer>().sprite;
            populationCounter.text = "Population: " + planetCheck.planetHovered.GetComponent<Planet>().population + " billion";

            MarketSwitchToMarketComms();
        }
    }

    public void Journal()
    {
        if (journalObject.activeInHierarchy == false)
        {
            CloseAllUI();
        }

        if (playerInput.currentActionMap.name == "UI")
        {
            if (playerMovement.isPlayerPiloting)
            {
                playerInput.SwitchCurrentActionMap("PlayerShip");
            }
            else
            {
                playerInput.SwitchCurrentActionMap("PlayerSuit");
            }
        }
        else
        {
            playerInput.SwitchCurrentActionMap("UI");
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

        if (playerInput.currentActionMap.name == "UI")
        {
            if (playerMovement.isPlayerPiloting)
            {
                playerInput.SwitchCurrentActionMap("PlayerShip");
            }
            else
            {
                playerInput.SwitchCurrentActionMap("PlayerSuit");
            }
        }
        else
        {
            playerInput.SwitchCurrentActionMap("UI");
        }

        SelectFirstItemHolder();

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

    public void PauseMenu()
    {
        if (pauseMenu.activeInHierarchy == false)
        {
            CloseAllUI();
        }

        if (playerInput.currentActionMap.name == "UI")
        {
            if (playerMovement.isPlayerPiloting)
            {
                playerInput.SwitchCurrentActionMap("PlayerShip");
            }
            else
            {
                playerInput.SwitchCurrentActionMap("PlayerSuit");
            }
        }
        else
        {
            playerInput.SwitchCurrentActionMap("UI");
        }

        pauseMenu.SetActive(!pauseMenu.activeInHierarchy);

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedInPauseMenu);

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            Debug.Log("Turned off ");
        }
        else
        {
            Time.timeScale = 0;
            Debug.Log("Turned off ");
        }

        if (pauseMenu.activeInHierarchy)
        {
            isInUI = true;
        }
        else
        {
            isInUI = false;
        }
    }

    public void TriggerNextDialogue()
    {
        triggeredNextStep = true;
    }

    //Function to return back to main menu
    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        playerInput.SwitchCurrentActionMap("MainMenu");
        GameManager.instance.LoadMainMenu();
    }

    //Selects the first item in the inventory for gamepad use
    public void SelectFirstItemHolder()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if(inv.items.Count > 0)
        {
            if (inv.uiItemHolders.Where(x => inv.itemsEquipped.Contains(x.itemHeld) != true).Count() > 0)
            {
                EventSystem.current.SetSelectedGameObject(inv.uiItemHolders.Where(x => inv.itemsEquipped.Contains(x.itemHeld) != true).First().gameObject);
                Debug.Log("Currently selected is " + EventSystem.current.gameObject.name);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(inv.uiItemHolders[0].gameObject);
                Debug.Log("Currently selected is " + EventSystem.current.gameObject.name);
            }
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(invisSelectButton);
        }
    }

    //Displays active quest in the quest log
    public void DrawQuest(Quest quest)
    {
        GameObject questDisplay = Instantiate(PrefabManager.instance.questDisplay, activeMissionsLog.transform);
        questDisplay.GetComponent<QuestDisplayHolder>().questHeld = quest;    
    }

    //Removes quest display from quest log
    public void RemoveQuest(Quest quest)
    {
        List<QuestDisplayHolder> list = activeMissionsLog.GetComponentsInChildren<QuestDisplayHolder>().Where(x => x.questHeld.id == quest.id).ToList();
        for (int i = 0; i < list.Count; i++)
        {
            Destroy(list[i].gameObject);
        }
    }
    #endregion

    #region AnimatedUI
    //Animates locaftion name display
    public void ToggleLocationNameDisplay(bool toggleBool, string locationName)
    {         
        CanvasGroup canv = mainStationDisplay.GetComponent<CanvasGroup>();
        LeanTween.cancel(canv.gameObject);

        if (toggleBool == true)
        {
            stationNameDisplay.text = "Entering " + locationName;
            LeanTween.cancel(canv.gameObject);
            LeanTween.alphaCanvas(canv, 1, 1);
            LeanTween.alphaCanvas(canv, 0, 1).setDelay(5f);
        }
        else if (toggleBool == false)
        {
            stationNameDisplay.text = "Leaving " + locationName;
            LeanTween.cancel(canv.gameObject);
            LeanTween.alphaCanvas(canv, 1, 1);
            LeanTween.alphaCanvas(canv, 0, 1).setDelay(1f);
        }
    }

    //Types the planet description for the planet interaction screen
    public void TypePlanetDescription(Planet planet, TMP_Text description)
    {
        StopAllCoroutines();
        description.text = "";
        StartCoroutine(TypeText(planet.planetDescription, description));
    }

    /// <summary>
    /// Animating typing to a specific text mesh pro component. Use UIManager.instance.DisplayDialogue() to 
    /// display to dialogue box instead.
    /// </summary>
    /// <param name="stringToType"></param>
    /// <param name="textToWriteIn"></param>
    public void AnimateTyping(string stringToType, TMP_Text textToWriteIn)
    {       
        StartCoroutine(TypeText(stringToType, textToWriteIn));
    }

    //Coroutine for animating the moving text in any text mesh pro writing box
    IEnumerator TypeText(string stringToType, TMP_Text writingBox)
    {
        writingBox.text = "";

        for (int i = 0; i < stringToType.Length - 1; i++)
        {
            char item = stringToType[i];

            writingBox.text += item;

            i++;

            item = stringToType[i];

            writingBox.text += item;

            audioSource.PlayOneShot(typingClip);
            yield return new WaitForSecondsRealtime(letterTypingPause);
        }
    }

    //Dialogue pages is the string you want it to display in the dialogue box, dialogue source is the place from where the dialogue came from
    public void DisplayDialogue(string dialoguePages, string dialogueSource)
    {
        StopAllCoroutines();
        dialogueDisplay.SetActive(true);
        dialogueDisplay.GetComponentsInChildren<TMP_Text>()[1].text = dialogueSource;
        //LeanTween.moveLocalY(dialogueDisplay, -400, 0.4f).setEaseInQuad();
        
        StartCoroutine(WriteDialogue(dialoguePages));        
    }

    //Coroutine for actually animating the writing of text in the dialogue box
    IEnumerator WriteDialogue(string dialoguePages)
    {
        AnimateTyping(dialoguePages, dialogueDisplay.GetComponentInChildren<TMP_Text>());

        yield return new WaitForSecondsRealtime(letterTypingPause * dialoguePages.Length);

        dialogueDisplay.SetActive(false);
        //LeanTween.moveLocalY(dialogueDisplay, -800, 0.4f).setEaseInQuad();
        yield return new WaitForSecondsRealtime(0);
    }

    //List overload for previous functions in order to have multiple pages of dialogue

    /// <summary>
    /// It displays dialogue with the source of the second argument in the dialogue page.
    /// Each element of the list is treated as a separate page, and will be displayed
    /// in the next iteration.
    /// </summary>
    /// <param name="dialoguePages"></param>
    /// <param name="dialogueSource"></param>
    public void DisplayDialogue(List<string> dialoguePages, string dialogueSource)
    {
        StopAllCoroutines();
        dialogueDisplay.SetActive(true);
        dialogueDisplay.GetComponentsInChildren<TMP_Text>()[1].text = dialogueSource;
        //LeanTween.moveLocalY(dialogueDisplay, -480, 0.4f).setEaseInQuad();

        StartCoroutine(WriteDialogue(dialoguePages));
    }

    //Coroutine for actually animating the writing of text in the dialogue box
    IEnumerator WriteDialogue(List<string> dialoguePages)
    {
        foreach (var item in dialoguePages)
        {
            AnimateTyping(item, dialogueDisplay.GetComponentInChildren<TMP_Text>());

            yield return new WaitForSecondsRealtime(letterTypingPause * item.Length);
        }
        dialogueDisplay.SetActive(false);

        //LeanTween.moveLocalY(dialogueDisplay, 60, 0.4f).setEaseInQuad();
        yield return new WaitForSecondsRealtime(0);
    }

    #endregion

    #region Marketstuff

    //Selects the item to buy in the market window
    public void MarketSelectCommodityInMarketWindow(BaseItem comm)
    {
        commSelectedInMarketWindow = comm;
        marketUnitSelector.maxValue = comm.itemStack;
        marketUnitSelector.value = comm.itemStack;
    }

    //Buys the commodity currently selected from the planet player interacted with.
    public void MarketBuyCommodity()
    {
        try
        {
            if(commSelectedInMarketWindow.itemStack >= commSelectedInMarketWindowUnits)
            {
                commSelectedInMarketWindow.itemStack -= commSelectedInMarketWindowUnits;
                BaseItem newCommToAdd = ScriptableObject.Instantiate(commSelectedInMarketWindow);
                newCommToAdd.itemStack = commSelectedInMarketWindowUnits;

                inv.AddItem(newCommToAdd);
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

                inv.AddItem(newCommToAdd);
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

    //Sells the commodity currently selected from the planet player interacted with
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
                player.GetComponent<Inventory>().items.Remove(player.GetComponent<Inventory>().items.Where(x => x == commSelectedInMarketWindow).First());
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

    //Run when the value in the slider of the planet interaction menu changes to change the display and select
    //how many items are selected of the current stack
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

    //Switches tab of planet interaction menu shop to market
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

    //Switches tab of planet interaction menu shop to inventory
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
    #endregion
}
