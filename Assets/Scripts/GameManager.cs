using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using FullSerializer;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject loadingScreen;
    public Image loadingBar;
    public TMP_Text fpsCounter;

    public string optionsFilePath = "/SpaceDudeOptions.ini";
    public string saveFilePath = "/SpaceDudeSave.spsv";
    public InputActionAsset inputActions;

    public OptionsSO options;
    public ProgressSave progressSave;

    private void Awake()
    {
        instance = this;
        optionsFilePath = Application.persistentDataPath + optionsFilePath;
        saveFilePath = Application.persistentDataPath + saveFilePath;

        HandleSaveInit();

        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);        
    }

    //Function to check if we should load current settings or create new ones for first startup
    public void HandleSaveInit()
    {
        if (File.Exists(optionsFilePath))
        {
            LoadOptions();
            Debug.Log("Loaded options from " + optionsFilePath);
        }
        else
        {
            Debug.Log("Created options " + optionsFilePath);
            options = new OptionsSO();
            SaveOptions();
        }

        if (File.Exists(saveFilePath))
        {
            LoadProgress();
        }
    }

    //Handles loading bar progress when loading scenes
    private void Update()
    {
        if (loadingScreen.activeSelf)
        {
            timer += Time.deltaTime;
            displayLoadingBarLoadingValue = Mathf.Lerp(0, totalSceneProgress, timer);
            loadingBar.fillAmount = displayLoadingBarLoadingValue;
        }

        if (fpsCounter.enabled)
        {
            fpsCounter.text = "FPS: " + Mathf.RoundToInt(1 / Time.deltaTime);
        }
    }

    #region Scenes

    //Loads the game scene from the main menu
    float timer, displayLoadingBarLoadingValue;
    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void LoadGame()
    {
        loadingScreen.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TITLE_SCREEN));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN_GAME, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    //Loads the main menu scene from anywhere else
    public void LoadMainMenu()
    {
        loadingScreen.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MAIN_GAME));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    //Calculates the progress of the scene loading - might need to change this later on as we generate more
    //stuff in the galaxy.
    float totalSceneProgress;
    public IEnumerator GetSceneLoadProgress()
    {
        for (int i = 0; i < scenesLoading.Count; i++)
        {
            while (!scenesLoading[i].isDone)
            {
                totalSceneProgress = 0;

                foreach (var item in scenesLoading)
                {
                    totalSceneProgress += item.progress;
                }

                totalSceneProgress = (totalSceneProgress / scenesLoading.Count);

                yield return null;
            }
        }

        loadingScreen.SetActive(false);
    }

    #endregion

    #region Saving
    public void SaveOptions()
    {
        string optionsJson = JsonUtility.ToJson(options);
        
        File.WriteAllText(optionsFilePath, optionsJson);
    }

    public void LoadOptions()
    {
        options = JsonUtility.FromJson<OptionsSO>(File.ReadAllText(optionsFilePath));
        inputActions.LoadBindingOverridesFromJson(options.keybindsJson);
    }

    public void SaveProgress()
    {
        string questString = Serialization.Serialize(progressSave.GetType(), progressSave);

        File.WriteAllText(saveFilePath, questString);
    }

    public void LoadProgress()
    {
        ProgressSave empty = new ProgressSave();
        progressSave = (ProgressSave)Serialization.Deserialize(empty.GetType(), File.ReadAllText(saveFilePath));
    }


    #endregion
}

//Enum to hold all the scenes we need for easy reference to switch between them.
public enum SceneIndexes
{
    MANAGER = 0,
    TITLE_SCREEN = 1,
    MAIN_GAME = 2
}
