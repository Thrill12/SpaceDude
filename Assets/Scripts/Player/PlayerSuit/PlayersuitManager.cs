using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;

public class PlayersuitManager : MonoBehaviour
{
    [Header("Player Character")] 
    public GameObject instantiatedPlayerSuit;
    public GameObject playerSuitCamPrefab;
    [Tooltip("The global volume for post processing which is enabled when controlling the player character.")]
    public GameObject suitVolume;

    [Header("Ship Spawn Points"), Tooltip("The spawn point in the ship interior where the player character is when the player leaves the cockpit and enters the main body of their ship.")]
    public GameObject playerSuitSpawn;
    [Tooltip("The external spawn point where the player is spawned when they leave out the ship's air lock which places them in space.")]
    public GameObject airLockExterior;
    [Tooltip("The internal location of the player when they enter the airlock.")]
    public GameObject airLockInterior;
    
    [HideInInspector]
    public GameObject instantiatedPlayerSuitCam;
    [HideInInspector]
    public PlayerShipMovement ship;

    private void Start()
    {
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam = Instantiate(playerSuitCamPrefab, transform.position, Quaternion.identity);
        instantiatedPlayerSuitCam.SetActive(false);
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = instantiatedPlayerSuit.transform;
        ship = GetComponent<PlayerShipMovement>();
    }

    public void PlayerLeaveCockpit()
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

    public void PlayerEnterCockpit()
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