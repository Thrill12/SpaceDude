using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;

public class PlayersuitManager : MonoBehaviour
{
    public GameObject instantiatedPlayerSuit;
    public GameObject playerSuitCamPrefab;
    public GameObject playerSuitSpawn;
    public GameObject suitVolume;   
    [HideInInspector]
    public GameObject instantiatedPlayerSuitCam;
    public PlayerShipMovement ship;

    private void Start()
    {
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam = Instantiate(playerSuitCamPrefab, transform.position, Quaternion.identity);
        instantiatedPlayerSuitCam.SetActive(false);
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = instantiatedPlayerSuit.transform;
        ship = GetComponent<PlayerShipMovement>();
    }

    public void PlayerLeaveShip()
    {
        instantiatedPlayerSuit.transform.position = playerSuitSpawn.transform.position;
        instantiatedPlayerSuit.SetActive(true);
        instantiatedPlayerSuitCam.SetActive(true);

        suitVolume.SetActive(true);

        playerSuitSpawn.GetComponent<SpriteRenderer>().enabled = true;
        playerSuitSpawn.GetComponent<Light2D>().enabled = true;

        ship.GetComponent<Rigidbody2D>().velocity *= 0;
        ship.GetComponent<Rigidbody2D>().mass = 100000;
        ship.isPlayerInShip = false;
        ship.followCam.gameObject.SetActive(false);
        ship.FoldInWings();
    }

    public void PlayerEnterShip()
    {
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam.SetActive(false);

        suitVolume.SetActive(false);

        ship.GetComponent<Rigidbody2D>().mass = 1;
        playerSuitSpawn.GetComponent<SpriteRenderer>().enabled = false;
        playerSuitSpawn.GetComponent<Light2D>().enabled = false;

        ship.isPlayerInShip = true;
        ship.followCam.gameObject.SetActive(true);
        ship.FoldOutWings();
    }
}