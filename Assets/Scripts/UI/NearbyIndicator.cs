using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class NearbyIndicator : MonoBehaviour
{
    [Header("Colours and radii")]

    public Color enemyColour;
    [Tooltip("Distance from the player object at which the arrow will float")]
    public float enemyRadius;

    [Space(5)]

    public Color playerColour;
    [Tooltip("Distance from the player object at which the arrow will float")]
    public float playerRadius;

    [Space(5)]

    public Color friendlyColour;
    [Tooltip("Distance from the player object at which the arrow will float")]
    public float friendlyRadius;

    [Space(5)]

    public Color neutralColour;
    [Tooltip("Distance from the player object at which the arrow will float")]
    public float neutralRadius;

    [HideInInspector]
    public GameObject playerObj;
    private GameObject objRotatingTo;

    [HideInInspector]
    public float maxDistance;

    [HideInInspector]
    public GameObject objToRotateTo;

    [HideInInspector]
    public float floatRadius;

    [HideInInspector]
    public float distanceBetweenPlayerAndTarget;

    private GameObject child;
    private Image arrowImage;

    private void Awake()
    {
        child = transform.GetChild(0).gameObject;
        arrowImage = child.GetComponent<Image>();
    }

    private void Update()
    {
        try
        {
            playerObj = GameObject.FindGameObjectWithTag("PlayerSuit");
        }
        catch
        {
            Debug.Log("Player not found in nearby indicator");
        }

        if (playerObj == null) return;        

        child.SetActive(playerObj.activeSelf);

        if (objRotatingTo != null)
        {
            distanceBetweenPlayerAndTarget = Vector2.Distance(playerObj.transform.position, objRotatingTo.transform.position);

            RotateToObject();          

            if (distanceBetweenPlayerAndTarget > maxDistance)
            {
                child.SetActive(false);
            }
            else
            {
                child.SetActive(true);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void RotateToObject()
    {
        if(playerObj == null) return;

        objRotatingTo = objToRotateTo;

        //Direction from the player object to the target.
        Vector3 dirToTarget = objRotatingTo.transform.position - playerObj.transform.position;
        dirToTarget = dirToTarget.normalized;

        float radius = 1f;

        if (objToRotateTo.CompareTag("Player"))
        {
            radius = playerRadius;
        }
        else if (objToRotateTo.CompareTag("Enemy"))
        {
            radius = enemyRadius;
        }
        else if (objToRotateTo.CompareTag("Friendly"))
        {
            radius = friendlyRadius;
        }
        else if (objToRotateTo.CompareTag("Neutral"))
        {
            radius = neutralRadius;
        }

        //Will align the arrow between the player and the target, at a distance of radius from the player
        child.transform.position = playerObj.transform.position + (dirToTarget * radius);        

        CheckIndicatorColours(objToRotateTo);

        //Rotating towards the object the arrow is targeting
        float angle = Mathf.Atan2(dirToTarget.y, dirToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle - 90, Vector3.forward);
        child.transform.rotation = q;
    }

    //Function to change the arrow's colour based on who they are following
    private void CheckIndicatorColours(GameObject objToRotateTo)
    {
        if (objToRotateTo.CompareTag("Player"))
        {
            Color col = playerColour;
            col.a = (1 - distanceBetweenPlayerAndTarget / maxDistance);
            arrowImage.color = col;
        }
        else if (objToRotateTo.CompareTag("Enemy"))
        {
            Color col = enemyColour;
            col.a = (1 - distanceBetweenPlayerAndTarget / maxDistance);
            arrowImage.color = col;
        }
        else if (objToRotateTo.CompareTag("Friendly"))
        {
            Color col = friendlyColour;
            col.a = (1 - distanceBetweenPlayerAndTarget / maxDistance);
            arrowImage.color = col;
        }
        else if (objToRotateTo.CompareTag("Neutral"))
        {
            Color col = neutralColour;
            col.a = (1 - distanceBetweenPlayerAndTarget / maxDistance);
            arrowImage.color = col;
        }
    }
}
