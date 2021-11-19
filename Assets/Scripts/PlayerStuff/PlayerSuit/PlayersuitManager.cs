using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PlayersuitManager : MonoBehaviour
{
    public GameObject playerSuitPrefab;
    public GameObject playerSuitCamPrefab;
    [HideInInspector]
    public GameObject instantiatedPlayerSuit;
    [HideInInspector]
    public GameObject instantiatedPlayerSuitCam;
    public PlayerShipMovement ship;

    private void Start()
    {
        instantiatedPlayerSuit = Instantiate(playerSuitPrefab, transform.position, Quaternion.identity);
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam = Instantiate(playerSuitCamPrefab, transform.position, Quaternion.identity);
        instantiatedPlayerSuitCam.SetActive(false);
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = instantiatedPlayerSuit.transform;
        ship = GetComponent<PlayerShipMovement>();
    }

    public void PlayerLeaveShip()
    {
        instantiatedPlayerSuit.transform.position = transform.position;
        instantiatedPlayerSuit.SetActive(true);
        instantiatedPlayerSuitCam.SetActive(true);
        ship.GetComponent<Rigidbody2D>().velocity *= 0;
        ship.isPlayerInShip = false;
        ship.followCam.gameObject.SetActive(false);
    }

    public void PlayerEnterShip()
    {
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam.SetActive(false);
        ship.isPlayerInShip = true;
        ship.followCam.gameObject.SetActive(true);
    }
}