using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour
{
	private float lengthX, lengthY, startPosX, startPosY;
	public GameObject cam;
	public float parallaxEffectX, parallaxEffectY, offsetX, offsetY;

    private void Start()
    {
        startPosX = transform.position.x;
        startPosY = transform.position.y;

        lengthX = GetComponent<SpriteRenderer>().bounds.size.x;
        lengthY = GetComponent<SpriteRenderer>().bounds.size.y;
    }

    private void Update()
    {
        float tempX = (cam.transform.position.x * (1 - parallaxEffectX));
        float tempY = (cam.transform.position.y * (1 - parallaxEffectY));

        float distX = cam.transform.position.x * parallaxEffectX;
        float distY = cam.transform.position.y * parallaxEffectY;

        transform.position = new Vector3(startPosX + distX, startPosY + distY, transform.position.z);

        if (tempX > startPosX + (lengthX - offsetX))
        {
            startPosX += lengthX;
        }
        else if (tempX < startPosX - (lengthX - offsetX))
        {
            startPosX -= lengthX;
        }

        if (tempY > startPosY + (lengthY - offsetY))
        {
            startPosY += lengthY;
        }
        else if (tempY < startPosY - (lengthY - offsetY))
        {
            startPosY -= lengthY;
        }
    }
}
