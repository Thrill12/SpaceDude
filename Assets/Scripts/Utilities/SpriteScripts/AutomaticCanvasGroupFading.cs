using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutomaticCanvasGroupFading : MonoBehaviour
{
    public float fadeDuration;

    private void Start()
    {
        LeanTween.alphaCanvas(GetComponent<CanvasGroup>(), 0, fadeDuration);
    }
}
