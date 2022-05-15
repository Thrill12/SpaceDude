using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadGameButton : MonoBehaviour
{
    public string saveGamePath;
    public void LoadSpecificGame()
    {
        GameManager.instance.LoadExistingSave(saveGamePath);
        MainMenu.instance.GoToSavedGame();
    }
}
