using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputActionAsset inputActionAsset;

    [Header("UI Selections")]
    public GameObject firstObjSelectedFromGameStart;
    public GameObject firstObjSelectedFromOptions;
    public GameObject firstObjSelectedInOptions;

    [Space(10)]

    [Header("Menus")]

    public GameObject optionsMenu;
    public GameObject mainMenu;
    public GameObject rebindingKeysKeyboard;
    public GameObject rebindingKeysGamePad;

    [Header("Settings")]

    public AudioMixer mixer;
    public GameObject masterVolumeBar;
    public GameObject musicVolumeBar;
    public GameObject sfxVolumeBar;

    private void Start()
    {
        SetSelectedObject(firstObjSelectedFromGameStart);    

        playerInput.SwitchCurrentActionMap("MainMenu");
        optionsMenu.SetActive(false);

        GameManager.instance.LoadOptions();
        InitializeSettings();
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
    }

    //Turns on options
    public void Options()
    {
        mainMenu.SetActive(false);
        SetSelectedObject(firstObjSelectedInOptions);
        optionsMenu.SetActive(true);
    }

    //Comes back from options
    public void BackFromOptions()
    {
        mainMenu.SetActive(true);
        SetSelectedObject(firstObjSelectedFromOptions);
        SaveSettings();
        optionsMenu.SetActive(false);
    }

    //Selects the object to be selected for gamepad use
    private void SetSelectedObject(GameObject objectToSelect)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectToSelect);
    }

    //Switches to game scene
    public void GoToGame()
    {
        SaveSettings();
        GameManager.instance.LoadGame();
    }

    //Loads settings from save file
    public void InitializeSettings()
    {
        OptionsSO options = GameManager.instance.options;

        masterVolumeBar.GetComponent<Image>().fillAmount = options.masterLevel;  
        musicVolumeBar.GetComponent<Image>().fillAmount = options.musicLevel;  
        sfxVolumeBar.GetComponent<Image>().fillAmount = options.sfxLevel;

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

        GameManager.instance.SaveOptions();
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
