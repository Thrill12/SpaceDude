using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundImage : MonoBehaviour
{
    public bool activated = true;

    private GameObject player;
    private SpriteRenderer rend;

    private Vector2 playerPos;
    private Vector2 currentPos;
   
    public float minStarSize = 250;
    public float minAsteroidSize = 5;
    public float minOtherSize = 50;
    public float minNebulaeSize = 5;

    public List<Sprite> stars;
    public List<Sprite> asteroids;
    public List<Sprite> other;
    public List<Sprite> nebulae;

    public TypeOfBackground type;

    public enum TypeOfBackground
    {
        Star,
        Asteroid,
        Other,
        Nebula
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        rend = GetComponent<SpriteRenderer>();

        if(type == TypeOfBackground.Asteroid)
        {
            rend.sprite = asteroids[Random.Range(0, asteroids.Count)];
            var size = Random.Range(minAsteroidSize, minAsteroidSize * 3);
            transform.localScale = new Vector2(size, size);
        }
        else if(type == TypeOfBackground.Star)
        {
            rend.sprite = stars[Random.Range(0, stars.Count)];
            var size = Random.Range(minStarSize, minStarSize * 3);
            transform.localScale = new Vector2(size, size);
        }
        else if(type == TypeOfBackground.Other)
        {
            rend.sprite = other[Random.Range(0, other.Count)];
            var size = Random.Range(minOtherSize, minOtherSize * 3);
            transform.localScale = new Vector2(size, size);
        }
        else if(type == TypeOfBackground.Nebula)
        {
            rend.sprite = nebulae[Random.Range(0, nebulae.Count)];
            var size = Random.Range(minNebulaeSize, minNebulaeSize * 3);
            transform.localScale = new Vector2(size, size);
        }

        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + transform.localScale.x);
        transform.localEulerAngles = new Vector3(0, 0, Random.Range(0, 360));
        rend.sortingOrder = Mathf.RoundToInt(-transform.position.z + rend.bounds.size.x);
        //rend.sortingOrder = Mathf.RoundToInt(transform.position.z);
    }

}
