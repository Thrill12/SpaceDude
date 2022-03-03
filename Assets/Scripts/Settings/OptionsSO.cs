using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[System.Serializable]
public class OptionsSO
{
    [Header("Sounds")]
    public float masterLevel = 0.6f;
    public float musicLevel = 0.6f;
    public float sfxLevel = 0.6f;

    [Header("Keybinds")]
    public string keybindsJson;

    [Header("Options")]
    public int qualityTier = 3;
    public int maxFPS = 60;
    public bool fpsCounter = false;
    public bool vsync = true;
}
