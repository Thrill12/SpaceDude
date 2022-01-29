using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkAsSurvivingSceneChange : MonoBehaviour
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
}
