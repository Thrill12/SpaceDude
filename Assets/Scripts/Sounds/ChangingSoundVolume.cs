using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class ChangingSoundVolume : MonoBehaviour
{
    public AudioMixer mixer;
    public Image volumeBar;

    public void IncrementVolume(string audioGroup)
    {
        volumeBar.fillAmount += 0.1f;
        mixer.SetFloat(audioGroup, Mathf.Log10(volumeBar.fillAmount) * 20);
    }

    public void DeincrementVolume(string audioGroup)
    {
        volumeBar.fillAmount -= 0.1f;
        if(volumeBar.fillAmount < 0.05f)
        {
            mixer.SetFloat(audioGroup, -80);
        }
        else
        {
            mixer.SetFloat(audioGroup, Mathf.Log10(volumeBar.fillAmount) * 20);
        }
        
    }
}
