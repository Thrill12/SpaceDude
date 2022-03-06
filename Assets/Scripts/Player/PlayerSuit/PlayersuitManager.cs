using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.Rendering.Universal;

//Manages the player suit, and sits on the ship as the player suit gets deactivated.
public class PlayersuitManager : MonoBehaviour
{
    [Header("Player Character")]
    public GameObject instantiatedPlayerSuit;
    public GameObject playerSuitCamPrefab;
    [Tooltip("The global volume for post processing which is enabled when controlling the player character.")]
    public GameObject suitVolume;

    public float orthoSizeNormal;
    public float orthoSizeDialogue;

    [Header("Ship Spawn Points"), Tooltip("The spawn point in the ship interior where the player character is when the player leaves the cockpit and enters the main body of their ship.")]
    public GameObject cockpitExitSpawn;
    [Tooltip("The external spawn point where the player is spawned when they leave out the ship's air lock which places them in space.")]
    public GameObject airLockExterior;
    [Tooltip("The internal location of the player when they enter the airlock.")]
    public GameObject airLockInterior;

    [Space(5)]

    public GameObject playerSuitUI;
    public GameObject shipUI;

    [HideInInspector]
    public GameObject instantiatedPlayerSuitCam;
    [HideInInspector]
    public PlayerShipMovement ship;

    public Animator fadeAnimation;
    public GameObject shipExt;
    public GameObject shipInt;

    private void Start()
    {
        #region Make player character inactive.
        //Hide the player character, setting the character game object to inactive as default the player is in the ship at start of the scene.
        instantiatedPlayerSuit = Inventory.instance.player.gameObject;
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam = Instantiate(playerSuitCamPrefab, transform.position, Quaternion.identity);
        instantiatedPlayerSuitCam.SetActive(false);
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = null;
        #endregion

        //Get the player ship.
        ship = GetComponent<PlayerShipMovement>();
    }

    private void Update()
    {
        if (UIManager.instance.playerInput.currentActionMap.name == "Dialogue")
        {
            instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = orthoSizeDialogue;
            instantiatedPlayerSuitCam.GetComponent<CinemachineCameraOffset>().m_Offset = Vector2.Lerp(instantiatedPlayerSuitCam.GetComponent<CinemachineCameraOffset>().m_Offset,
                new Vector3(), 1f);
        }
        else
        {
            instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = orthoSizeNormal;
            instantiatedPlayerSuitCam.GetComponent<CinemachineCameraOffset>().m_Offset = Vector2.Lerp(
                instantiatedPlayerSuitCam.GetComponent<CinemachineCameraOffset>().m_Offset, 
                ((instantiatedPlayerSuit.transform.position + instantiatedPlayerSuit.transform.up) - instantiatedPlayerSuit.transform.position).normalized,
                0.1f);
        }

        if (ship.isPlayerPiloting)
        {
            playerSuitUI.SetActive(false);
            shipUI.SetActive(true);
        }
        else
        {
            playerSuitUI.SetActive(true);
            shipUI.SetActive(false);
        }
    }

    public void PlayerLeaveCockpit()
    {
        //Fade screen
        fadeAnimation.SetTrigger("Fade");

        instantiatedPlayerSuitCam.SetActive(true);
        //Set the vCam to follow the ship.
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = ship.gameObject.transform;
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_LookAt = ship.gameObject.transform;

        instantiatedPlayerSuit.transform.SetParent(shipInt.transform, false);
        
        ship.newInput.SwitchCurrentActionMap("PlayerSuit");

        Invoke("LeaveCockpitTransition", .7f);
    }

    public void PlayerEnterCockpit()
    {
        //Hide the player character, setting the character game object to inactive as they are now controlling the player ship.
        instantiatedPlayerSuit.SetActive(false);
        instantiatedPlayerSuitCam.SetActive(false);
        //Disable the player character post processing volume.
        suitVolume.SetActive(false);

        //Set the ship follow cam to active.
        ship.followCam.gameObject.SetActive(true);

        //Fade screen
        fadeAnimation.SetTrigger("Fade");
        ship.newInput.SwitchCurrentActionMap("PlayerShip");

        Invoke("EnterCockpitTransition", .7f);
    }

    public void PlayerExitShip()
    {
        //Fade screen
        fadeAnimation.SetTrigger("Fade");

        instantiatedPlayerSuit.transform.parent = null;
        instantiatedPlayerSuit.GetComponent<PlayerEntity>().useOxygen = true;

        Invoke("ExitShipTransition", .7f);
    }
    public void PlayerEnterShip()
    {
        //Fold out the player ship's wings as the player is now piloting.
        ship.FoldOutWings();

        //Fade screen
        fadeAnimation.SetTrigger("Fade");

        instantiatedPlayerSuitCam.SetActive(true);
        //Set the vCam to follow the ship.
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = shipInt.gameObject.transform;
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_LookAt = shipInt.gameObject.transform;

        instantiatedPlayerSuit.GetComponent<PlayerEntity>().useOxygen = false;

        Invoke("EntertShipTransition", .7f);
    }

    public void LeaveCockpitTransition()
    {
        //Play the transition to show the ship interior.

        //Move the player to the outside of the cockpit wihtin the ship.
        instantiatedPlayerSuit.transform.position = cockpitExitSpawn.transform.position;
        //Set the plater character objects to be active as the polayer is now going to control the player cahracter.
        instantiatedPlayerSuit.SetActive(true);
        instantiatedPlayerSuitCam.SetActive(true);
        //Enable the player character post processing volume.
        suitVolume.SetActive(true);
        //Set the vCam to follow the ship.
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = shipInt.gameObject.transform;
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 0f;
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 0f;

        #region Update Ship
        ship.isPlayerPiloting = false;
        //Think this is to make the ship stop??
        //ship.GetComponent<Rigidbody2D>().drag = ship.drag;
        //Makes the engine thrusters inactive.
        //ship.thrusterParticleSpawnRate = 0;
        //Turns the lights on the ship down.
        ship.leftLight.intensity = 0;
        ship.rightLight.intensity = 0;
        #endregion

        shipInt.SetActive(true);
        shipExt.SetActive(false);
    }
    public void EnterCockpitTransition()
    {
        //Play the transition to show the ship exterior.

        //Set the vCam to follow the player.
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = ship.gameObject.transform;
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_LookAt = null;

        shipInt.SetActive(false);
        shipExt.SetActive(true);

        //Update the ship.
        ship.isPlayerPiloting = true;
    }

    public void ExitShipTransition()
    {
        //Move the player to the outside of the cockpit wihtin the ship.
        instantiatedPlayerSuit.transform.position = airLockExterior.transform.position;
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = instantiatedPlayerSuit.transform;
        shipInt.SetActive(false);
        shipExt.SetActive(true);

        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_XDamping = 1;
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineFramingTransposer>().m_YDamping = 1;

        //Fold out the player ship's wings as the player is now piloting.
        ship.FoldInWings();
    }

    public void EntertShipTransition()
    {
        //Move the player to the outside of the cockpit wihtin the ship.
        instantiatedPlayerSuit.transform.position = airLockInterior.transform.position;
        //Set the plater character objects to be active as the polayer is now going to control the player cahracter.
        instantiatedPlayerSuit.SetActive(true);
        instantiatedPlayerSuitCam.SetActive(true);
        //Enable the player character post processing volume.
        suitVolume.SetActive(true);
        //Set the vCam to follow the ship.
        instantiatedPlayerSuitCam.GetComponent<CinemachineVirtualCamera>().m_Follow = shipInt.gameObject.transform;


        shipInt.SetActive(true);
        shipExt.SetActive(false);
    }
}