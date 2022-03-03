using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject loadingScreen;
    public Image loadingBar;
    public TMP_Text fpsCounter;

    [Header("Paths")]

    public string optionsFilePath = "/SpaceDudeOptions.ini";
    public string saveFilePath = "/SpaceDudeSave.spsv";

    [Space(15)]

    public InputActionAsset inputActions;

    public OptionsSO options;
    public ProgressSave progressSave;

    public float autosaveInterval = 300;
    private float autosavingTimer = 0;

    private void Awake()
    {
        instance = this;
        optionsFilePath = Application.persistentDataPath + optionsFilePath;
        saveFilePath = Application.persistentDataPath + saveFilePath;
        
        SceneManager.LoadSceneAsync((int)SceneIndexes.TITLE_SCREEN, LoadSceneMode.Additive);

        HandleSaveInit();     
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
        else
        {
            progressSave = new ProgressSave();
            progressSave.npcStates = new NPCStatesSave();
            progressSave.questsSaved = new QuestsSavedSO();
            progressSave.inventorySave = new InventorySave();
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

        if (fpsCounter.gameObject.activeInHierarchy)
        {
            fpsCounter.text = "FPS: " + Mathf.RoundToInt(1 / Time.deltaTime);
        }

        autosavingTimer += Time.deltaTime;

        if(autosavingTimer >= autosaveInterval)
        {
            autosavingTimer = 0;
            SaveProgress();
        }
    }

    #region LoadingResources

    //Function to get the files required for hte item to enable them to be saved
    public void LoadResourcesForItem(BaseItem item)
    {
        item.itemIcon = Resources.Load<Sprite>(item.itemIconPath);

        if (item as BaseEquippable)
        {
            if (item as BaseWeapon)
            {
                BaseWeapon weapon = item as BaseWeapon;
                weapon.weaponObject = Resources.Load<GameObject>(weapon.weaponObjectPath);
                weapon.attackSound = Resources.Load<AudioClip>(weapon.attackSoundPath);

                if (item as BaseGun)
                {
                    BaseGun gun = item as BaseGun;
                    gun.projectile = Resources.Load<GameObject>(gun.projectilePath);
                    gun.outOfAmmoSound = Resources.Load<AudioClip>(gun.outOfAmmoSoundFilePath);
                }
            }
        }
    }

    public void LoadResourcesForQuest(Quest quest)
    {
        quest.questCompletedGraph = Resources.Load<DialogueGraph>(quest.questCompletedGraphPath);
        quest.questInProgressGraph = Resources.Load<DialogueGraph>(quest.questInProgressGraphPath);
    }

    #endregion

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
        SaveProgress();
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

        fpsCounter.gameObject.SetActive(options.fpsCounter);

        inputActions.LoadBindingOverridesFromJson(options.keybindsJson);

        Application.targetFrameRate = options.maxFPS;

        if (options.vsync)
        {
            QualitySettings.vSyncCount = 1;
        }
        else
        {
            QualitySettings.vSyncCount = 0;
        }
    }

    public void SaveNPCStates()
    {
        if (NPCManager.instance == null) return;

        NPCStatesSave save = new NPCStatesSave();

        foreach (var item in NPCManager.instance.allNPCs)
        {
            save.npcList.Add(item);
        }

        progressSave.npcStates = save;
    }

    public void LoadNPCStates()
    {
        if (NPCManager.instance == null) return;

        ProgressSave progressSavee = progressSave as ProgressSave;
        NPCStatesSave save = progressSavee.npcStates as NPCStatesSave;

        foreach (var item in save.npcList)
        {
            NPCManager.instance.allNPCs.Add(item);
        }
    }

    public void SaveInventory()
    {
        if (Inventory.instance == null) return;

        progressSave.inventorySave = new InventorySave();

        List<BaseItem> itemsEquipped = new List<BaseItem>();
        List<BaseItem> playerItems = new List<BaseItem>();
        List<BaseItem> shipItems = new List<BaseItem>();

        for (int i = 0; i < Inventory.instance.itemsEquipped.Count; i++)
        {
            BaseItem item = Inventory.instance.itemsEquipped[i];
            itemsEquipped.Add(item);
        }

        for (int i = 0; i < Inventory.instance.playerInventoryItems.Count; i++)
        {
            BaseItem item = Inventory.instance.playerInventoryItems[i];
            playerItems.Add(item);
        }

        for (int i = 0; i < Inventory.instance.shipInventoryItems.Count; i++)
        {
            BaseItem item = Inventory.instance.shipInventoryItems[i];
            shipItems.Add(item);
        }

        progressSave.inventorySave.itemsEquipped = itemsEquipped.ToArray();
        progressSave.inventorySave.playerInventoryItems = playerItems.ToArray();
        progressSave.inventorySave.shipInventoryItems = shipItems.ToArray();
    }

    public void SavePlayerLocations()
    {
        progressSave.playerShipLocation = PlayerShipMovement.instance.gameObject.transform.position;
        progressSave.playerShipRotation = PlayerShipMovement.instance.gameObject.transform.rotation.eulerAngles;
    }

    public void LoadPlayerLocations()
    {
        PlayerShipMovement.instance.gameObject.transform.position = progressSave.playerShipLocation;
        PlayerShipMovement.instance.gameObject.transform.rotation = Quaternion.Euler(progressSave.playerShipRotation);
    }

    public void SaveProgress()
    {
        SaveNPCStates();
        SaveInventory();
        SavePlayerLocations();

        string json = Serialization.Serialize(progressSave.GetType(), progressSave);
        File.WriteAllText(saveFilePath, json);
    }

    public void LoadProgress()
    {
        Debug.Log("Loading progress Data");

        progressSave = Serialization.Deserialize(progressSave.GetType(), File.ReadAllText(saveFilePath)) as ProgressSave;

        LoadNPCStates();
    }

    //public void SaveProgress()
    //{
    //    SaveNPCStates();
    //    SaveInventory();

    //    Serialization.Serialize(progressSave, saveFilePath);
    //}

    //public void LoadProgress()
    //{
    //    Debug.Log("Loading progress Data");

    //    progressSave = Serialization.Deserialize(File.ReadAllText(saveFilePath)) as ProgressSave;

    //    LoadNPCStates();
    //}

    #endregion
}

//Enum to hold all the scenes we need for easy reference to switch between them.
public enum SceneIndexes
{
    MANAGER = 0,
    TITLE_SCREEN = 1,
    MAIN_GAME = 2
}
