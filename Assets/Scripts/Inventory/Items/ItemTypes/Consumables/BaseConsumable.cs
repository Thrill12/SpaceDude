using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseConsumable : GeneralItem
{
    public AudioClip useSound;
    [SerializeField]
    public string useSoundPath;

    public virtual void Use()
    {
        itemStack -= 1;
        if(useSound != null)
        {
            GameManager.instance.uiManager.audioSource.PlayOneShot(useSound);
        }
    }
}
