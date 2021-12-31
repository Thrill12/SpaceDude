using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class StarBackgroundGeneration : MonoBehaviour
{
    public GameObject backImage;

    [Space(5)]

    public float minPosZStar = 200;
    public float maxPosZStar = 1500;

    [Space(5)]

    public float minPosZAsteroid = 200;
    public float maxPosZAsteroid = 1500;

    [Space(5)]

    public float minPosZOther = 200;
    public float maxPosZOther = 1500;

    [Space(5)]

    public float size = 500;

    [Space(5)]

    public int numOfStars = 10;
    public int numOfAsteroids = 500;
    public int numOfOther = 10;

    private void Start()
    {
        for (int i = 0; i < numOfStars; i++)
        {           
            var z = Random.Range(minPosZStar, maxPosZStar);

            GameObject star = Instantiate(backImage, new Vector3(0, 0, z), Quaternion.identity);

            star.transform.position = Random.insideUnitSphere * size;
            star.transform.position = new Vector3(star.transform.position.x, star.transform.position.y, Mathf.Abs(star.transform.position.z));
            star.GetComponent<BackgroundImage>().type = BackgroundImage.TypeOfBackground.Star;
            star.transform.parent = transform;
        }

        for (int i = 0; i < numOfAsteroids; i++)
        {
            var z = Random.Range(minPosZStar, maxPosZStar);

            GameObject asteroid = Instantiate(backImage, new Vector3(0, 0, z), Quaternion.identity);

            asteroid.transform.position = Random.insideUnitCircle * size;
            asteroid.transform.position = new Vector3(asteroid.transform.position.x, asteroid.transform.position.y, z);

            asteroid.GetComponent<BackgroundImage>().type = BackgroundImage.TypeOfBackground.Asteroid;
            asteroid.transform.parent = transform;
        }

        for (int i = 0; i < numOfOther; i++)
        {
            var z = Random.Range(minPosZStar, maxPosZStar);

            GameObject star = Instantiate(backImage, new Vector3(0, 0, z), Quaternion.identity);

            star.transform.position = Random.insideUnitCircle * size;
            star.transform.position = new Vector3(star.transform.position.x, star.transform.position.y, z);

            star.GetComponent<BackgroundImage>().type = BackgroundImage.TypeOfBackground.Other;
            star.transform.parent = transform;
        }
    }
}
