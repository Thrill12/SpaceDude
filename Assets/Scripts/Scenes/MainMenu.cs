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

    public void Options()
    {
        mainMenu.SetActive(false);
        SetSelectedObject(firstObjSelectedInOptions);
        optionsMenu.SetActive(true);
    }

    public void BackFromOptions()
    {
        mainMenu.SetActive(true);
        SetSelectedObject(firstObjSelectedFromOptions);
        SaveSettings();
        optionsMenu.SetActive(false);
    }

    private void SetSelectedObject(GameObject objectToSelect)
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(objectToSelect);
    }

    public void GoToGame()
    {
        SaveSettings();
        GameManager.instance.LoadGame();
    }

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

    public void SaveSettings()
    {
        OptionsSO options = GameManager.instance.options;

        options.masterLevel = masterVolumeBar.GetComponent<Image>().fillAmount;
        options.musicLevel = musicVolumeBar.GetComponent<Image>().fillAmount;
        options.sfxLevel = sfxVolumeBar.GetComponent<Image>().fillAmount;

        options.keybindsJson = inputActionAsset.SaveBindingOverridesAsJson();

        GameManager.instance.SaveOptions();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
    }

    public void QuitGame()
    {
        SaveSettings();
        Application.Quit();
    }
}
