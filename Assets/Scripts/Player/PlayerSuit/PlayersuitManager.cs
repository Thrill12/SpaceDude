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

        ship.isPlayerInShip = false;
        ship.followCam.gameObject.SetActive(false);
        ship.FoldInWings();
        ship.GetComponent<Rigidbody2D>().drag = ship.drag;
        ship.thrusterParticleSpawnRate = 0;
        ship.leftLight.intensity = 0;
        ship.rightLight.intensity = 0;
    }

    public void PlayerEnterShip()
    {
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam.SetActive(false);

        suitVolume.SetActive(false);

        playerSuitSpawn.GetComponent<SpriteRenderer>().enabled = false;
        playerSuitSpawn.GetComponent<Light2D>().enabled = false;

        ship.isPlayerInShip = true;
        ship.followCam.gameObject.SetActive(true);
        ship.FoldOutWings();
    }
}