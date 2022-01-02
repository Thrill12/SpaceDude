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
    public GameObject cockpitExitSpawn;
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
        #region Make player character inactive.
        //Hide the player character, setting the character game object to inactive as default the player is in the ship at start of the scene.
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam = Instantiate(playerSuitCamPrefab, transform.position, Quaternion.identity);
        instantiatedPlayerSuitCam.SetActive(false);
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = instantiatedPlayerSuit.transform;
        #endregion

        //Get the player ship.
        ship = GetComponent<PlayerShipMovement>();
    }

    public void PlayerLeaveCockpit()
    {
        //Move the player to the outside of the cockpit wihtin the ship.
        instantiatedPlayerSuit.transform.position = cockpitExitSpawn.transform.position;
        //Set the plater character objects to be active as the polayer is now going to control the player cahracter.
        instantiatedPlayerSuit.SetActive(true);
        instantiatedPlayerSuitCam.SetActive(true);
        //Enable the player character post processing volume.
        suitVolume.SetActive(true);

        #region Update Ship
        ship.isPlayerPiloting = false;
        //Make the following virtual camera for the ship inactive.
        ship.followCam.gameObject.SetActive(false);
        //Fold in the wings on the ship as the player has stopped piloting.
        ship.FoldInWings();
        //Think this is to make the ship stop??
        ship.GetComponent<Rigidbody2D>().drag = ship.drag;
        //Makes the engine thrusters inactive.
        ship.thrusterParticleSpawnRate = 0;
        //Turns the lights on the ship down.
        ship.leftLight.intensity = 0;
        ship.rightLight.intensity = 0;
        #endregion
    }

    public void PlayerEnterCockpit()
    {
        //Hide the player character, setting the character game object to inactive as they are now controlling the player ship.
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam.SetActive(false);
        //Disable the player character post processing volume.
        suitVolume.SetActive(false);

        //Update the ship.
        ship.isPlayerPiloting = true;
        //Set the ship follow cam to active.
        ship.followCam.gameObject.SetActive(true);
        //Fold out the player ship's wings as the player is now piloting.
        ship.FoldOutWings();
    }
}