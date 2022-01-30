using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidFieldAsteroid : MonoBehaviour
{
    public List<Sprite> asteroidSprites;

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = asteroidSprites[Random.Range(0, asteroidSprites.Count)];
        Destroy(GetComponent<PolygonCollider2D>());
        gameObject.AddComponent<PolygonCollider2D>();
        transform.position = new Vector3(transform.position.x, transform.position.y, 0);
    }
}
