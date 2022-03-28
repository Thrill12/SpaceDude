using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System.Reflection;
using System;
using System.Globalization;

public class UIManager : MonoBehaviour
{
    [Header("Sounds")]

    public AudioClip normalClickSound;
    public AudioClip itemEquipSound;
    public AudioClip itemUnequipSound;
    public AudioClip enterMenuSound;

    [Space(5)]

    public PlayerEntity playerEntity;

    public GameObject firstSelectedInPauseMenu;
    public PlayerInput playerInput;
    public AudioListener audioListener;

    [Header("In-Game UI")]

    public TMP_Text speedIndicator;
    public TMP_Text deathScreenMiniText;
    public List<string> deathScreenMiniTextList;
    public GameObject respawnButton;

    public Image dampenersImage;

    public GameObject deathScreen;

    public GameObject playerSuitUI;
    public GameObject shipUI;

    [Space(5)]

    public bool isWarning = false;
    public GameObject warningObject;
    public TMP_Text warningTextObject;
    public AudioClip warningSound;

    [Header("Dialogue Stuff")]

    [HideInInspector]
    public bool triggeredNextStep;
    public GameObject dialogueDisplay;
    public GameObject dialogueLocationEnabled;
    public GameObject dialogueLocationDisabled;

    public GameObject choiceOption;
    public GameObject choiceDisplayer;

    [Space(10)]

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
    public GameObject characterSlotsDisplay;
    public GameObject shipInventory;
    public GameObject pauseMenu;

    public bool isInShipInventory;

    [Header("Inventory")]

    public ItemStatDisplayer leftItemStatDisplay;
    public ItemStatDisplayer rightItemStatDisplay;
    public GameObject characterStatDisplayPrefab;
    public GameObject characterStatDisplay;
    public GameObject invisSelectButton;
    public BaseItem currentlySelectedItemToDisplay;

    [Space(5)]

    [Header("Discoveries")]

    public GameObject discoveryRightSide;
    public GameObject discoveryHexHolder;
    public TMP_Text discoveryName;
    public TMP_Text discoveryDescription;
    public TMP_Text discoveryType;

    [Space(15)]

    public GameObject playerHitEffectHolder;

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
    public PlayerInventory inv;

    public bool isInUI;

    [Space(5)]

    public AudioSource audioSource;
    public AudioClip typingClip;
    public float letterTypingPause;

    private List<GameObject> drawnCharacterStats = new List<GameObject>();

    private void Awake()
    {
        GameManager.instance.LoadEssentials();
    }

    private void Start()
    {
        Time.timeScale = 1;
        GameManager.instance.masterMixer.SetFloat("EQMaster", 1);

        pfManager = GameObject.FindGameObjectWithTag("PrefabManager").GetComponent<PrefabManager>();
        planetCheck = player.GetComponent<PlanetCheckerRaycast>();

        foreach (var item in GameObject.FindGameObjectsWithTag("Planet"))
        {
            allPlanets.Add(item.GetComponent<Planet>());
        }

        audioSource = GetComponent<AudioSource>();
        originalDialogueDisplayPosition = dialogueDisplay.transform.position;

        playerInput.SwitchCurrentActionMap("PlayerShip");

        audioSource.ignoreListenerPause = true;

        MusicManager.instance.shouldLoop = false;
    }

    private void Update()
    {
        if (isInUI)
        {
            MusicManager.instance.musicSource.Pause();
            MusicManager.instance.overrideSound = true;

            playerSuitUI.SetActive(false);
            shipUI.SetActive(false);
            stationNameDisplay.gameObject.SetActive(false);
            GameManager.instance.fpsCounter.gameObject.SetActive(false);
        }
        else
        {
            if (!MusicManager.instance.musicSource.isPlaying)
            {
                MusicManager.instance.musicSource.Play();
                MusicManager.instance.overrideSound = false;
            }          

            GameManager.instance.fpsCounter.gameObject.SetActive(true);
            stationNameDisplay.gameObject.SetActive(true);
        }

        isInShipInventory = shipInventory.activeInHierarchy;

        //Displaying the currently hovered on item in the inventory
        if (playerInput.currentControlScheme == "GamePad")
        {
            //Displaying the current item selected in the inventory
            if (currentlySelectedItemToDisplay != null && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.GetComponent<UIItemHolder>())
            {
                if (currentlySelectedItemToDisplay as BaseEquippable)
                {
                    BaseEquippable equip = (BaseEquippable)currentlySelectedItemToDisplay;

                    if (equip.isEquipped || inv.shipInventoryItems.Contains(equip))
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
                    if (inv.shipInventoryItems.Contains(currentlySelectedItemToDisplay))
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

                    if (equip.isEquipped || inv.shipInventoryItems.Contains(equip))
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
                    if(inv.shipInventoryItems.Contains(currentlySelectedItemToDisplay))
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
            speedIndicator.text = UnityEngine.Random.Range(1000, 10000000) + " m/s";       
        }       

        if (playerMovement.dampeners)
        {
            dampenersImage.enabled = true;
        }
        else
        {
            dampenersImage.enabled = false;
        }

        if (inventory.activeInHierarchy)
        {
            DrawCharacterStats();
        }        

        #region RandomCheckStuff

        if(commSelectedInMarketWindow == null)
        {
            marketUnitSelector.value = 0;
            marketUnitSelectorText.text = "";
        }

        #endregion
    }   

    public void DisplayDeathScreen()
    {
        deathScreen.SetActive(true);
        deathScreenMiniText.text = deathScreenMiniTextList[UnityEngine.Random.Range(0, deathScreenMiniTextList.Count)];
        LeanTween.alphaCanvas(deathScreen.GetComponent<CanvasGroup>(), 1, 2).setIgnoreTimeScale(true);
        playerInput.SwitchCurrentActionMap("UI");
        SelectUIObject(respawnButton);
    }

    private void DisplayItemStatsInItemStatDisplayer()
    {
        if (shipInventory.activeInHierarchy)
        {
            if (inv.playerInventoryItems.Contains(currentlySelectedItemToDisplay))
            {
                leftItemStatDisplay.gameObject.SetActive(false);
                rightItemStatDisplay.gameObject.SetActive(true);
                rightItemStatDisplay.GetComponent<ItemStatDisplayer>().ShowItem(currentlySelectedItemToDisplay);
            }
            else
            {
                leftItemStatDisplay.gameObject.SetActive(true);
                rightItemStatDisplay.gameObject.SetActive(false);
                leftItemStatDisplay.GetComponent<ItemStatDisplayer>().ShowItem(currentlySelectedItemToDisplay);
            }
        }
        else
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
    }

    #region NormalUI

    public void SelectUIObject(GameObject objToSelect = null)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objToSelect);

        if(objToSelect != null)
        {
            audioSource.PlayOneShot(normalClickSound);
        }       
    }

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

    //Turns on the planet interaction menu when the player is in the ship
    public void PlanetDescription(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        PlanetDescription();    
    }

    //Turns on the journal menu for the player to keep track of their progress
    public void Journal(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        Journal();
    }

    //Turns on player inventory when the player is outside the ship
    public void Inventory(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        Inventory();
    }    

    //Turns on pause menu and pauses the game
    public void PauseMenu(InputAction.CallbackContext context)
    {
        if (context.phase != InputActionPhase.Started) return;

        PauseMenu();
    }

    //Overloads for no context
    public void CloseAllUI()
    {
        leftItemStatDisplay.gameObject.SetActive(false);
        rightItemStatDisplay.gameObject.SetActive(false);
        currentlySelectedItemToDisplay = null;
        SelectUIObject(null);

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

        if (shipInventory.activeInHierarchy)
        {
            ShipInventory();
        }

        isInUI = false;

        PauseOff();

        if (playerMovement.isPlayerPiloting)
        {
            playerInput.SwitchCurrentActionMap("PlayerShip");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("PlayerSuit");
        }

        audioSource.PlayOneShot(enterMenuSound);
    }

    public void PlanetDescription()
    {
        if(planetCheck.planetHoveredP != null)
        {
            Debug.Log(planetCheck.planetHoveredP.planetName);
            //ProgressHolder.instance.AddPlanet(planetCheck.planetHovered);

            Debug.Log("Were inside bois");

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
                LeanTween.scale(planetUI, new Vector3(1, 1, 1), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();
            }
            else
            {
                isInUI = false;
                LeanTween.scale(planetUI, new Vector3(0, 0, 0), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();
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
            SelectUIObject(discoveryHexHolder.transform.GetChild(0).gameObject);
            journalObject.transform.localScale = new Vector3(0, 0, 0);
            LeanTween.scale(journalObject, new Vector3(1,1,1), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();
        }
        else
        {
            isInUI = false;
            LeanTween.scale(journalObject, new Vector3(0, 0, 0), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();
        }
    }

    public void DisplayDiscoverySelected(ProgressHex hex)
    {
        discoveryRightSide.SetActive(true);

        if (hex.isDiscovered)
        {
            discoveryDescription.text = hex.hexDescription;
            discoveryName.text = hex.hexName;
            discoveryType.text = hex.hexType.ToString();
        }
        else
        {
            discoveryDescription.text = "???";
            discoveryName.text = "???";
            discoveryType.text = "???";
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
        characterSlotsDisplay.SetActive(true);
        shipInventory.SetActive(false);

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
            inventory.transform.localScale = new Vector3(0, 0, 0);
            LeanTween.scale(inventory, new Vector3(1, 1, 1), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();
        }
        else
        {
            isInUI = false;
            LeanTween.scale(inventory, new Vector3(0, 0, 0), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();
        }

        leftItemStatDisplay.gameObject.SetActive(false);
        rightItemStatDisplay.gameObject.SetActive(false);
    }

    public void ShipInventory()
    {
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
        shipInventory.SetActive(!shipInventory.activeInHierarchy);
        characterSlotsDisplay.SetActive(false);

        if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }
        else
        {
            Time.timeScale = 0;
        }

        if (shipInventory.activeInHierarchy)
        {
            isInUI = true;
            commSelectedInMarketWindow = null;
            commSelectedInMarketWindowUnits = 0;
            inventory.transform.localScale = new Vector3(0, 0, 0);
            LeanTween.scale(inventory, new Vector3(1, 1, 1), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();
        }
        else
        {
            isInUI = false;
            leftItemStatDisplay.gameObject.SetActive(false);
            rightItemStatDisplay.gameObject.SetActive(false);
            LeanTween.scale(inventory, new Vector3(0, 0, 0), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();
        }
    }

    public void PauseMenu()
    {
        if (!pauseMenu.activeSelf)
        {
            PauseOn();
        }
        else
        {
            PauseOff();
        }
    }

    public void PauseOn()
    {
        CloseAllUI();

        playerInput.SwitchCurrentActionMap("UI");

        pauseMenu.SetActive(true);
        pauseMenu.transform.localScale = new Vector3(0, 0, 0);
        LeanTween.scale(pauseMenu, new Vector3(1, 1, 1), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedInPauseMenu);

        Time.timeScale = 0;

        isInUI = true;
    }

    public void PauseOff()
    {
        if (playerMovement.isPlayerPiloting)
        {
            playerInput.SwitchCurrentActionMap("PlayerShip");
        }
        else
        {
            playerInput.SwitchCurrentActionMap("PlayerSuit");
        }

        LeanTween.scale(pauseMenu, new Vector3(0, 0, 0), 0.1f).setIgnoreTimeScale(true).setEaseOutCubic();

        pauseMenu.SetActive(false);

        Time.timeScale = 1;

        isInUI = false;
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

    public void LoadLatestSave()
    {
        playerInput.SwitchCurrentActionMap("MainMenu");
        GameManager.instance.LoadLatestSave();
    }

    //Selects the first item in the inventory for gamepad use
    public void SelectFirstItemHolder()
    {
        EventSystem.current.SetSelectedGameObject(null);

        if(inv.playerInventoryItems.Count > 0)
        {
            if (inv.uiItemHolders.Where(x => inv.itemsEquipped.Contains(x.itemHeld) != true).Count() > 0)
            {
                EventSystem.current.SetSelectedGameObject(inv.uiItemHolders.Where(x => inv.itemsEquipped.Contains(x.itemHeld) != true).First().gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(invisSelectButton);
        }
    }

    private void DrawCharacterStats()
    {
        foreach (var item in drawnCharacterStats)
        {
            Destroy(item);
        }

        GameManager.instance.playerInventory.playerCreditsText.text = GameManager.instance.playerInventory.playerCredits.ToString();
        GameManager.instance.playerInventory.playerPowerText.text = GameManager.instance.playerInventory.playerPower.ToString();

        List<string> itemStatsStrings = playerEntity.GetType().GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
                .Where(x => x.FieldType == typeof(Stat)).Select(x => x.Name).ToList();

        List<Stat> allItemStats = new List<Stat>();

        foreach (var str in itemStatsStrings)
        {
            allItemStats.Add(GetFieldValue<Stat>(playerEntity, str));
        }

        allItemStats.Reverse();

        foreach (var stat in allItemStats)
        {
            GameObject displayer = Instantiate(characterStatDisplayPrefab, characterStatDisplay.transform);
            TMP_Text statText = displayer.GetComponent<TMP_Text>();

            TextInfo info = new CultureInfo("en-US", false).TextInfo;

            statText.text = stat.Value + " " + stat.statName;

            drawnCharacterStats.Add(displayer);
        }
    }

    private static T GetFieldValue<T>(object obj, string fieldName)
    {
        if (obj == null)
            throw new ArgumentNullException("obj");

        var field = obj.GetType().GetField(fieldName, BindingFlags.Public |
                                                      BindingFlags.NonPublic |
                                                      BindingFlags.Instance);

        if (field == null)
            throw new ArgumentException("fieldName", "No such field was found.");

        if (!typeof(T).IsAssignableFrom(field.FieldType))
            throw new InvalidOperationException("Field type and requested type are not compatible.");

        return (T)field.GetValue(obj);
    }

    //Displays active quest in the quest log
    public void DrawQuest(Quest quest)
    {
        if (activeMissionsLog.GetComponentsInChildren<QuestDisplayHolder>().Any(x => x.questHeld.QuestName == quest.QuestName)) return;

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

    #region DialogueChoices
    [HideInInspector]
    public bool hasChosenDialogueChoice = false;
    [HideInInspector]
    public int chosenDialogueChoice;
    //Pass in a list of strings to display as choices in each button, returns an int to
    //tell the node parser what choice they chose
    public void DisplayChoices(List<string> choices)
    {
        playerInput.SwitchCurrentActionMap("UI");

        choiceDisplayer.SetActive(true);

        string firstItem = choices[0];
        GameObject firstOption = Instantiate(choiceOption, choiceDisplayer.transform);
        firstOption.GetComponentInChildren<TMP_Text>().text = firstItem;
        firstOption.GetComponent<Button>().onClick.AddListener(() => SelectChoice(0));

        if(playerInput.currentControlScheme == "GamePad")
        {
            SelectUIObject(firstOption);
        }

        for (int i = 1; i < choices.Count; i++)
        {
            string itemm = choices[i];
            GameObject choice = Instantiate(choiceOption, choiceDisplayer.transform);
            choice.GetComponentInChildren<TMP_Text>().text = itemm;
            choice.GetComponent<Button>().onClick.AddListener(() => SelectChoice(choices.IndexOf(itemm)));
        }
    }

    //Function called by the buttons spawned in for choosing choices - each button
    //is assigned an integer value with which we figure out the intended response
    //to the previous dialogue
    public int SelectChoice(int choiceNumber)
    {
        Debug.Log("Chose number " + choiceNumber);
        chosenDialogueChoice = choiceNumber;
        hasChosenDialogueChoice = true;

        choiceDisplayer.SetActive(false);

        for (int i = 1; i < choiceDisplayer.transform.childCount; i++)
        {
            Destroy(choiceDisplayer.transform.GetChild(i).gameObject);
        }

        return choiceNumber;
    }
    #endregion

    //Dialogue pages is the string you want it to display in the dialogue box, dialogue source is the place from where the dialogue came from
    public void DisplayDialogue(string dialoguePages, NPC dialogueSource)
    {
        StopAllCoroutines();
        dialogueDisplay.SetActive(true);
        dialogueDisplay.GetComponentsInChildren<TMP_Text>()[1].text = dialogueSource.npcName;
        dialogueDisplay.transform.Find("SpeakerImageBorder").transform.Find("SpeakerImageMask").transform.Find("SpeakerImage").GetComponent<Image>().sprite = dialogueSource.npcPortrait;
        //LeanTween.moveLocalY(dialogueDisplay, -400, 0.4f).setEaseInQuad();
        
        StartCoroutine(WriteDialogue(dialoguePages));        
    }

    //Coroutine for actually animating the writing of text in the dialogue box
    IEnumerator WriteDialogue(string dialoguePages)
    {
        AnimateDialogueIn();
        AnimateTyping(dialoguePages, dialogueDisplay.GetComponentInChildren<TMP_Text>());

        yield return new WaitForSecondsRealtime(letterTypingPause * dialoguePages.Length);

        dialogueDisplay.SetActive(false);
        //LeanTween.moveLocalY(dialogueDisplay, -800, 0.4f).setEaseInQuad();
        yield return new WaitForSecondsRealtime(0);
    }

    public void AnimateDialogueIn()
    {
        LeanTween.scaleY(dialogueDisplay, 1, 0.1f);
        LeanTween.scaleX(dialogueDisplay, 1, 0.1f).setDelay(0.1f);
    }

    public void AnimateDialogueOut()
    {
        LeanTween.scaleY(dialogueDisplay, 0.2f, 0.1f);
        LeanTween.scaleX(dialogueDisplay, 0.5f, 0.1f).setDelay(0.1f);
    }

    public void ShowWarning(string warningText)
    {       
        StartCoroutine(AnimateWarning(warningText));
    }

    public IEnumerator AnimateWarning(string text)
    {
        if (!isWarning)
        {
            isWarning = true;
            warningObject.GetComponent<CanvasGroup>().alpha = 1;
            warningTextObject.text = "Warning: " + text;
            warningObject.transform.localScale = new Vector3(1, 1, 1);

            audioSource.PlayOneShot(warningSound);

            LeanTween.alphaCanvas(warningObject.GetComponent<CanvasGroup>(), 0, 3);            

            yield return new WaitForSecondsRealtime(2.5f);

            LeanTween.scaleY(warningObject, 0, 0.2f);

            yield return new WaitForSecondsRealtime(0.5f);
            isWarning = false;
        }
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
                player.GetComponent<PlayerInventory>().playerInventoryItems.Remove(player.GetComponent<PlayerInventory>().playerInventoryItems.Where(x => x == commSelectedInMarketWindow).First());
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

        foreach (BaseItem item in player.GetComponent<PlayerInventory>().playerInventoryItems)
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

