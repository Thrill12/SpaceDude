using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldSpawner : MonoBehaviour
{
    public int minNumOfAsteroids;
    public int maxNumOfAsteroids;

    public GameObject asteroidPrefab;

    private void Start()
    {
        SpawnAsteroids();
    }

    void SpawnAsteroids()
    {
        for (int i = 0; i < Random.Range(minNumOfAsteroids, maxNumOfAsteroids); i++)
        {
            GameObject asteroid = Instantiate(asteroidPrefab, (Vector2)transform.position + Random.insideUnitCircle * GetComponent<CircleCollider2D>().radius, Quaternion.identity);
            asteroid.transform.SetParent(transform, true);
        }
    }
}
