using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;
    public PlayerInput playerInput;
    public InputActionAsset inputActionAsset;

    [Header("UI Selections")]
    public GameObject firstObjSelectedFromGameStart;
    public GameObject firstObjSelectedFromOptions;
    public GameObject firstObjSelectedInOptions;
    public GameObject firstObjSelectedFromLoadGameMenu;
    public GameObject firstObjSelectedInLoadGameMenu;

    [Space(10)]

    [Header("Menus")]

    public GameObject menuCamera;
    public GameObject optionsMenu;
    public GameObject mainMenu;
    public GameObject rebindingKeysKeyboard;
    public GameObject rebindingKeysGamePad;

    [Space(10)]

    [Header("Save Games")]

    public GameObject loadGameMenu;
    public GameObject saveGameDisplayerPrefab;
    public GameObject saveGameDisplayer;

    [Space(10)]

    [Header("Settings")]

    public AudioMixer mixer;
    public GameObject masterVolumeBar;
    public GameObject musicVolumeBar;
    public GameObject sfxVolumeBar;
    public TMP_Text graphicsTierText;
    public TMP_Text maxFPSText;
    public Toggle fpsCounter;
    public Toggle vsyncToggle;

    private bool isAnimating = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        SetSelectedObject(firstObjSelectedFromGameStart);    

        playerInput.SwitchCurrentActionMap("MainMenu");
        optionsMenu.SetActive(false);

        InitializeSettings();

        GameManager.instance.masterMixer = mixer;

        Time.timeScale = 1;
    }

    private void Update()
    {
        if (optionsMenu.activeSelf)
        {
            //Checks whether it should display the gamepad or keyboard rebinding icons/text
            if(playerInput.currentControlScheme == "GamePad")
            {
                rebindingKeysGamePad.SetActive(true);
                rebindingKeysKeyboard.SetActive(false);
            }
            else
            {
                rebindingKeysGamePad.SetActive(false);
                rebindingKeysKeyboard.SetActive(true);
            }
        }

        GameManager.instance.fpsCounter.gameObject.SetActive(GameManager.instance.options.fpsCounter);
    }

    public void DisplaySaveGames()
    {
        List<string> saves = GameManager.instance.GetAllSaveFiles();

        foreach (string save in saves)
        {
            GameObject display = Instantiate(saveGameDisplayerPrefab, saveGameDisplayer.transform);
            var splitString = save.Split(@"\");
            string strToDisplay = splitString[splitString.Length - 1];
            splitString = strToDisplay.Split(".");
            strToDisplay = splitString[0];
            display.GetComponentInChildren<TMP_Text>().text = strToDisplay.Replace("-", " ");
            display.GetComponentInChildren<LoadGameButton>().saveGamePath = save;
        }
    }

    //Turns on options
    public void Options()
    {
        if (isAnimating) return;
        StartCoroutine(OptionsCor());
    }

    public void LoadGameMenu()
    {
        if (isAnimating) return;
        StartCoroutine(LoadGameMenuCor());
    }

    private IEnumerator LoadGameMenuCor()
    {
        isAnimating = true;
        SetSelectedObject(firstObjSelectedInLoadGameMenu);
        loadGameMenu.SetActive(true);

        foreach (var item in saveGameDisplayer.GetComponentsInChildren<LoadGameButton>())
        {
            Destroy(item.gameObject);
        }

        LeanTween.rotate(menuCamera, new Vector3(0, 90, 0), 0.2f).setEaseOutCubic();

        yield return new WaitForSeconds(0.3f);

        mainMenu.SetActive(false);

        isAnimating = false;
        DisplaySaveGames();
    }

    private IEnumerator OptionsCor()
    {
        isAnimating = true;
        SetSelectedObject(firstObjSelectedInOptions);
        optionsMenu.SetActive(true);
        LeanTween.rotate(menuCamera, new Vector3(0, -90, 0), 0.2f).setEaseOutCubic();

        yield return new WaitForSeconds(0.3f);

        mainMenu.SetActive(false);

        isAnimating = false;
    }

    //Comes back from options
    public void BackFromOptions()
    {
        if (isAnimating) return;
        StartCoroutine(BackFromOptionsCor());        
    }

    public void BackFromLoadingGameMenu()
    {
        if(isAnimating) return;
        StartCoroutine(BackFromLoadingGameMenuCor());
    }

    private IEnumerator BackFromOptionsCor()
    {
        isAnimating = true;
        mainMenu.SetActive(true);
        SetSelectedObject(firstObjSelectedFromOptions);
        SaveSettings();
        LeanTween.rotate(menuCamera, new Vector3(0, 0, 0), 0.2f).setEaseOutCubic();

        yield return new WaitForSeconds(0.3f);

        optionsMenu.SetActive(false);
        isAnimating = false;
    }

    private IEnumerator BackFromLoadingGameMenuCor()
    {
        isAnimating = true;
        mainMenu.SetActive(true);
        SetSelectedObject(firstObjSelectedFromLoadGameMenu);
        
        LeanTween.rotate(menuCamera, new Vector3(0, 0, 0), 0.2f).setEaseOutCubic();

        yield return new WaitForSeconds(0.3f);

        loadGameMenu.SetActive(false);
        isAnimating = false;
    }

    //Selects the object to be selected for gamepad use
    private void SetSelectedObject(GameObject objectToSelect)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectToSelect);
    }

    //Switches to game scene
    public void GoToFreshGame()
    {
        if (isAnimating) return;
        GameManager.instance.StartNewSave();
        StartCoroutine(GoGameCor());
    }

    public void GoToSavedGame()
    {
        if(isAnimating) return;
        StartCoroutine(GoGameCor());
    }

    private IEnumerator GoGameCor()
    {
        isAnimating = true;
        SaveSettings();

        LeanTween.rotate(menuCamera, new Vector3(10, 0, 0), 1);

        yield return new WaitForSeconds(2.5f);
      
        GameManager.instance.LoadGameFromMainMenu();
    }

    //Loads settings from save file
    public void InitializeSettings()
    {
        OptionsSO options = GameManager.instance.options;

        masterVolumeBar.GetComponent<Image>().fillAmount = options.masterLevel;  
        musicVolumeBar.GetComponent<Image>().fillAmount = options.musicLevel;
        sfxVolumeBar.GetComponent<Image>().fillAmount = options.sfxLevel;

        QualitySettings.SetQualityLevel(options.qualityTier);

        graphicsTierText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
        maxFPSText.text = Application.targetFrameRate.ToString();

        fpsCounter.isOn = GameManager.instance.options.fpsCounter;
        vsyncToggle.isOn = GameManager.instance.options.fpsCounter;

        mixer.SetFloat("Master", Mathf.Log10(masterVolumeBar.GetComponent<Image>().fillAmount) * 20);
        mixer.SetFloat("Music", Mathf.Log10(musicVolumeBar.GetComponent<Image>().fillAmount) * 20);
        mixer.SetFloat("SFX", Mathf.Log10(sfxVolumeBar.GetComponent<Image>().fillAmount) * 20);
    }

    //Save settings to save file, found in GameManager.cs
    public void SaveSettings()
    {
        OptionsSO options = GameManager.instance.options;

        options.masterLevel = masterVolumeBar.GetComponent<Image>().fillAmount;
        options.musicLevel = musicVolumeBar.GetComponent<Image>().fillAmount;
        options.sfxLevel = sfxVolumeBar.GetComponent<Image>().fillAmount;
        options.keybindsJson = inputActionAsset.SaveBindingOverridesAsJson();
        options.fpsCounter = fpsCounter.isOn;

        GameManager.instance.SaveOptions();
    }

    public void IncreaseGraphics()
    {
        QualitySettings.IncreaseLevel(true);

        graphicsTierText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
        GameManager.instance.options.qualityTier = QualitySettings.GetQualityLevel();

        SaveSettings();
    }

    public void DecreaseGraphics()
    {
        QualitySettings.DecreaseLevel(true);

        graphicsTierText.text = QualitySettings.names[QualitySettings.GetQualityLevel()];
        GameManager.instance.options.qualityTier = QualitySettings.GetQualityLevel();

        SaveSettings();
    }

    public void IncreaseMaxFPS()
    {
        Application.targetFrameRate += 10;
        maxFPSText.text = Application.targetFrameRate.ToString();
        GameManager.instance.options.maxFPS = Application.targetFrameRate;

        SaveSettings();
    }
    
    public void DecreaseMaxFPS()
    {
        Application.targetFrameRate -= 10;
        maxFPSText.text = Application.targetFrameRate.ToString();
        GameManager.instance.options.maxFPS = Application.targetFrameRate;

        SaveSettings();
    }

    public void ToggleFPSCounter()
    {
        GameManager.instance.options.fpsCounter = !GameManager.instance.options.fpsCounter;
        SaveSettings();
    }

    public void ToggleVSync()
    {
        GameManager.instance.options.vsync = !GameManager.instance.options.vsync;
        if (GameManager.instance.options.vsync)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
        SaveSettings();
    }

    //Saves settings on application quit in case people do ALT+F4
    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    //Quits the game normally
    public void QuitGame()
    {
        SaveSettings();
        Application.Quit();
    }
}
