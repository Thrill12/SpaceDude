using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneStuff : MonoBehaviour
{
    public static SceneStuff instance;

    public List<GameObject> objectsToBeTurnedOnAtGameStart;

    private void Awake()
    {
        instance = this;
    }

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }
}
