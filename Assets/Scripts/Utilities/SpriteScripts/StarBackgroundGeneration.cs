using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Random = UnityEngine.Random;

public class StarBackgroundGeneration : MonoBehaviour
{
    public StarBackgroundGeneration instance;

    public int seed = 1;
    public bool randomizeSeed = true;

    [Space(5)]

    public GameObject backImage;

    [Space(5)]

    public float minPosZStar = 200;
    public float maxPosZStar = 1500;

    [Space(5)]

    public GameObject asteroidField;

    [Space(5)]

    public float minPosZOther = 200;
    public float maxPosZOther = 1500;

    [Space(5)]

    public float minPosZNebulae = 500;
    public float maxPosZNebulae = 1000;

    [Space(5)]

    public float size = 500;

    [Space(5)]

    public int numOfStars = 10;
    public int numOfAsteroids = 500;
    public int numOfOther = 10;
    public int numOfNebulae = 50;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        if (randomizeSeed)
        {
            seed = Random.Range(0, 10000000);
        }

        Random.InitState(seed);

        GenerateStars();
        GenerateAsteroids();
        GenerateMysteriousOneOffObjects();
        GenerateNebulae();
    }

    private void GenerateNebulae()
    {
        for (int i = 0; i < numOfNebulae; i++)
        {
            var z = Random.Range(minPosZNebulae, maxPosZNebulae);

            GameObject nebula = Instantiate(backImage, new Vector3(0, 0, z), Quaternion.identity);

            nebula.transform.position = Random.insideUnitCircle * size;
            nebula.transform.position = new Vector3(nebula.transform.position.x, nebula.transform.position.y, z);

            nebula.GetComponent<BackgroundImage>().type = BackgroundImage.TypeOfBackground.Nebula;
            Color col = nebula.GetComponent<SpriteRenderer>().color;
            col.a = 0.6f;
            nebula.GetComponent<SpriteRenderer>().color = col;
            nebula.transform.parent = transform;
        }
    }

    private void GenerateMysteriousOneOffObjects()
    {
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

    private void GenerateAsteroids()
    {
        for (int i = 0; i < numOfAsteroids; i++)
        {
            GameObject asteroid = Instantiate(asteroidField, Random.insideUnitCircle * size, Quaternion.identity);

            asteroid.transform.parent = transform;
        }
    }

    private void GenerateStars()
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
    }
}
