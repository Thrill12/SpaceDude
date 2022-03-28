using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class PlayerShipGPS : MonoBehaviour
{
    public string outputtedPosition;

    public TMP_Text bearingText;
    public TMP_Text positionText;
    public GameObject bearingHolder;

    private Transform playerShip;
    private Vector2 roundedPosition;

    private void Start()
    {
        playerShip = transform;
    }

    private void Update()
    {
        roundedPosition.x = Mathf.Ceil(playerShip.transform.position.x);
        roundedPosition.y = Mathf.Ceil(playerShip.transform.position.y);        

        if(Physics2D.BoxCastAll(roundedPosition, new Vector2(100, 100), 90, Vector2.up).Any(x => x.transform.gameObject.GetComponent<Planet>()))
        {
            Planet nearby = Physics2D.BoxCastAll(roundedPosition, new Vector2(100, 100), 90, Vector2.up).First(x => x.transform.gameObject.GetComponent<Planet>()).transform.gameObject.GetComponent<Planet>();
            outputtedPosition = roundedPosition.x + $"{nearby.planetName[0]}/" + roundedPosition.y + $"{nearby.planetName[0]}";
        }
        else
        {
            outputtedPosition = roundedPosition.x + "/" + roundedPosition.y;
        }

        positionText.text = outputtedPosition;

        bearingHolder.transform.localEulerAngles = new Vector3(0, 0, playerShip.transform.localEulerAngles.z );
        bearingText.text = Mathf.RoundToInt(playerShip.transform.localEulerAngles.z).ToString();
    }
}
