using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomLoadingIconSelector : MonoBehaviour
{
    private Image loadingImage;

    public float animationStep = 0.1f;
    public List<SmallPlanetSpriteList> loadingSprites = new List<SmallPlanetSpriteList>();

    private List<Sprite> chosenSprites = new List<Sprite>();

    private void Start()
    {
        loadingImage = GetComponent<Image>();

        chosenSprites = loadingSprites[Random.Range(0, loadingSprites.Count)].sprites;

        StartCoroutine(StartIteratingThroughPictures(animationStep));
    }

    IEnumerator StartIteratingThroughPictures(float timeStep)
    {
        int index = 0;
        while (gameObject.activeSelf)
        {
            if (index == chosenSprites.Count - 1)
            {
                index = 0;
            }
            
            loadingImage.sprite = chosenSprites[index];

            index++;
            yield return new WaitForSecondsRealtime(timeStep);
        }
    }
}

[System.Serializable]
public class SmallPlanetSpriteList
{
    public List<Sprite> sprites;
}
