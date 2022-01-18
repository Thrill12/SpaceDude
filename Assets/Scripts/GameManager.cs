using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject loadingScreen;
    public Image loadingBar;

    public string saveFile = "/SpaceDudeOptions.spsv";
    public InputActionAsset inputActions;
    public OptionsSO options;  

    private void Awake()
    {
        instance = this;
        saveFile = Application.persistentDataPath + saveFile;

        HandleSaveInit();

        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);        
    }

    //Function to check if we should load current settings or create new ones for first startup
    public void HandleSaveInit()
    {
        if (File.Exists(saveFile))
        {
            LoadOptions();
            Debug.Log("Loaded options from " + saveFile);
        }
        else
        {
            Debug.Log("Created options " + saveFile);
            options = new OptionsSO();
            SaveOptions();
        }
    }

    private void Update()
    {
        if (loadingScreen.activeSelf)
        {
            timer += Time.deltaTime;
            displayLoadingBarLoadingValue = Mathf.Lerp(0, totalSceneProgress, timer);
            loadingBar.fillAmount = displayLoadingBarLoadingValue;
        }
    }

    #region Scenes

    float timer, displayLoadingBarLoadingValue;
    List<AsyncOperation> scenesLoading = new List<AsyncOperation>();
    public void LoadGame()
    {
        loadingScreen.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.TITLE_SCREEN));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN_GAME, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

    public void LoadMainMenu()
    {
        loadingScreen.SetActive(true);
        scenesLoading.Add(SceneManager.UnloadSceneAsync((int)SceneIndexes.MAIN_GAME));
        scenesLoading.Add(SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive));

        StartCoroutine(GetSceneLoadProgress());
    }

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
        File.WriteAllText(saveFile, optionsJson);
    }

    public void LoadOptions()
    {
        options = JsonUtility.FromJson<OptionsSO>(File.ReadAllText(saveFile));
        inputActions.LoadBindingOverridesFromJson(options.keybindsJson);
    }
    #endregion    
}

public enum SceneIndexes
{
    MANAGER = 0,
    TITLE_SCREEN = 1,
    MAIN_GAME = 2
}
