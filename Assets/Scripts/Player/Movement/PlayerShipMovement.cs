using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.VFX;

public class PlayerShipMovement : MonoBehaviour
{
    public static PlayerShipMovement instance;

    public bool isPlayerPiloting = true;

    [Space(5)]

    [Header("Warp settings")]
    public Volume postProcessingVolume;
    public VisualEffect shipWarpEffectParticles;
    public float warpAcceleration = 1;
    public float minSpeedForWarp = 200;
    public float maxWarpSpeed;
    public float warpNavTurnSpeed = 1;
    public int maxWarpEffectRate = 500;
    public bool canWarp;
    public bool inWarp = false;
    public bool isChargingWarp = false;
    public int aspiteRequiredForJump;

    public Image fuelLevelImage;
    public TMP_Text fuelLevelText;
    public GameObject fuelLevel;

    [Space(5)]

    public AudioClip warpInSound;
    public AudioClip warpOutSound;
    public AudioSource warpSource;

    [Space(5)]

    [Header("Camera Settings")]
    public CinemachineVirtualCamera followCam;
    public AnimationCurve normalCamSizeCurve;
    public CinemachineVirtualCamera warpCamera;

    [Header("Movement Settings")]

    public float moveSpeed;
    private float baseMoveSpeed;
    public float baseAcceleration;
    [HideInInspector]
    public float accelAcceleration;
    
    public float maxSpeed = 1000;
    public float maxAngVel = 100;
    public AnimationCurve dragCurve;
    public float angularDrag = 5;
    public AnimationCurve thrusterSoundCurve;

    [Space(5)]
    public float rotationSpeed;

    [Space(10)]
    public GameObject rightWing;
    public GameObject leftWing;

    [Space(5)]

    public PlayerInput newInput;

    private Rigidbody2D rb;

    private Vector2 posOfMouseOnWorld;

    public bool dampeners = true;
    private float inputX;
    private float inputY;
    private Vector2 mousePos;

    [HideInInspector]
    public float currentSpeed;

    private PlayersuitManager playerSuit;

    public GameObject leftThrust;
    public GameObject rightThrust;
    public Light2D leftLight;
    public Light2D rightLight;

    private AudioSource src;
    public AudioSource wingsAudioSource;
    public AudioClip wingsFoldSound;

    private float lastVol = 0;
    private float currentVol = 0;

    [HideInInspector]
    public UIManager ui;
    [HideInInspector]
    public int thrusterParticleSpawnRate = 0;

    private bool isTweening = false;
    private bool isUISpeedNumberRandom = false;
    private int aspiteStack;

    private void Awake()
    {
        instance = this;
        GameManager.instance.LoadPlayerLocations();
    }

    private void Start()
    {
        accelAcceleration = baseAcceleration;
        shipWarpEffectParticles.SetInt(Shader.PropertyToID("Spawn Rate"), 0);
        isPlayerPiloting = true;
        rb = GetComponent<Rigidbody2D>();
        baseMoveSpeed = moveSpeed;
        src = GetComponent<AudioSource>();
        ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
        playerSuit = GetComponent<PlayersuitManager>();
        postProcessingVolume = GameObject.FindGameObjectWithTag("Volume").GetComponent<Volume>();
        //Setting up input actions for the new input system
        
    }

    private void Update()
    {
        if (ui.isInUI) return;

        ChangeParticleThrustRate(thrusterParticleSpawnRate);

        currentSpeed = rb.velocity.magnitude;

        thrusterParticleSpawnRate = (int)Mathf.Floor(currentSpeed * 250);

        if (GameManager.instance.playerInventory == null) return;

        if(GameManager.instance.playerInventory.shipInventoryItems.Any(x => x.itemType == ItemType.Aspite))
        {
            aspiteStack = GameManager.instance.playerInventory.shipInventoryItems.Where(x => x.itemType == ItemType.Aspite).Sum(x => x.itemStack);
        }     

        if (currentSpeed > minSpeedForWarp && aspiteStack > aspiteRequiredForJump)
        {
            canWarp = true;
        }
        else
        {
            canWarp = false;
        }

        if (thrusterParticleSpawnRate > 5 && !inWarp)
        {
            leftLight.intensity = 0.5f + (thrusterParticleSpawnRate / (maxSpeed * 250));
            rightLight.intensity = 0.5f + (thrusterParticleSpawnRate / (maxSpeed * 250));
        }
        else
        {
            leftLight.intensity = 0;
            rightLight.intensity = 0;
        }

        if (!isPlayerPiloting)
        {
            inputY = 0;
            inputX = 0;

            fuelLevel.SetActive(false);
        }
        else
        {
            fuelLevel.SetActive(true);
            
            if(aspiteStack / aspiteRequiredForJump > 9)
            {
                fuelLevelText.text = "Jumps available: 9+";
                fuelLevelImage.fillAmount = 1;
            }
            else
            {
                fuelLevelText.text = "Jumps available: " + Mathf.Floor(aspiteStack / aspiteRequiredForJump);
                fuelLevelImage.fillAmount = ((float)aspiteStack / (float)aspiteRequiredForJump) / (float)9;
            }
        }

        ManageThrustVolume(0.001f);

        if (!isPlayerPiloting) return;

     

        //if (Input.GetMouseButton(0))
        //{
        //    posOfMouseOnWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //}
        //mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);              

        ChangeCameraZoomVelocity();
    }

    //Checking for inputs and running their specific events/functions
    #region Inputs
    public void ChangeMovementVector(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            inputY = context.ReadValue<float>();
        }
        else
        {
            inputY = 0;
        }
    }

    public void ChangeTurning(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed)
        {
            inputX = context.ReadValue<float>();
        }
        else
        {
            inputX = 0;
        }
    }

    public void LeaveCockpit(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!inWarp && !isChargingWarp)
            {
                //if (currentSpeed / maxSpeed * 100 < 20)
                playerSuit.PlayerLeaveCockpit();
            }
        }
    }

    public void CheckInputsForWarp(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            if (!inWarp && !isChargingWarp && canWarp)
            {
                ActivateWarp();
            }
            else if (inWarp && !isChargingWarp)
            {
                DropOutWarp();
            }
        }
    }

    public void ToggleDampeners(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            dampeners = !dampeners;
        }
    }
    #endregion

    public void LeaveCockpit()
    {
        playerSuit.PlayerLeaveCockpit();
    }

    //Potential script to make ship face an objective if we add google maps drive by
    private void TurnToObjective(Vector3 positionToNavigateTo)
    {
        float angle = Mathf.Atan2(positionToNavigateTo.y - transform.position.y, positionToNavigateTo.x - transform.position.x) * Mathf.Rad2Deg;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, warpNavTurnSpeed);
    }

    //The warp is charging up and will launch when ready
    private void ActivateWarp()
    {
        rb.freezeRotation = true;
        shipWarpEffectParticles.SetInt(Shader.PropertyToID("Spawn Rate"), maxWarpEffectRate / 2);
        isChargingWarp = true;
        GameManager.instance.playerInventory.shipInventoryItems.Where(x => x.itemType == ItemType.Aspite).First().itemStack -= aspiteRequiredForJump;
        warpSource.PlayOneShot(warpInSound);
        GetComponent<CinemachineImpulseSource>().GenerateImpulse();
        Invoke("DropInWarp", 1.6f);
    }

    //Starts the warp and travels very fast
    private void DropInWarp()
    {
        isChargingWarp = false;
        inWarp = true;
        warpCamera.Priority = 1000;        

        foreach (var item in GetComponentsInChildren<Collider2D>().Where(x => x.isTrigger != true))
        {
            item.enabled = false;
        }

        GetComponent<CinemachineImpulseSource>().GenerateImpulse();

        LensDistortion distortion = null;
        postProcessingVolume.profile.TryGet<LensDistortion>(out distortion);
        distortion.active = true;

        StartCoroutine(IncreaseSpeedInWarp());
        StartCoroutine(IncreaseBlurAmount());
        StartCoroutine(IncreaseWarpEffectParticleRate());     
    }
    
    //Turns off warp and returns to normal
    private void DropOutWarp()
    {
        inWarp = false;
        warpCamera.Priority = 0;

        shipWarpEffectParticles.SetInt(Shader.PropertyToID("Spawn Rate"), 0);

        foreach (var item in GetComponentsInChildren<Collider2D>().Where(x => x.isTrigger != true))
        {
            item.enabled = true;
        }

        MotionBlur blur = null;
        postProcessingVolume.profile.TryGet<MotionBlur>(out blur);
        blur.intensity.value = 0;

        LensDistortion distortion = null;
        postProcessingVolume.profile.TryGet<LensDistortion>(out distortion);
        distortion.active = false;

        warpSource.PlayOneShot(warpOutSound);
        rb.freezeRotation = false;
        rb.velocity = rb.velocity.normalized * maxSpeed * 0.2f;
    }

    //Gradually increase the amount of particles released by the warp effect
    IEnumerator IncreaseWarpEffectParticleRate()
    {
        while(shipWarpEffectParticles.GetInt(Shader.PropertyToID("Spawn Rate")) < maxWarpEffectRate && inWarp)
        {
            shipWarpEffectParticles.SetInt(Shader.PropertyToID("Spawn Rate"), shipWarpEffectParticles.GetInt(Shader.PropertyToID("Spawn Rate")) + 1);

            yield return new WaitForSeconds(0.005f);
        }
    }

    //Gradually inrease the blur amount in the effect
    IEnumerator IncreaseBlurAmount()
    {
        MotionBlur blur = null;

        postProcessingVolume.profile.TryGet<MotionBlur>(out blur);

        while (blur.intensity.value < 0.15f && inWarp)
        {
            blur.intensity.value += 0.001f;
            yield return new WaitForSeconds(0.001f);
        }
    }

    IEnumerator IncreaseSpeedInWarp()
    {      
        while (inWarp && currentSpeed < maxWarpSpeed)
        {
            rb.velocity = -transform.up * warpAcceleration * (rb.velocity.magnitude + 1);
            yield return new WaitForSeconds(0.01f);
        }   
    }

    private void FixedUpdate()
    {
        if (isPlayerPiloting)
        {
            MoveAndTurn();
        }
    }

    public void ChangeCameraZoomVelocity()
    {
        float newSize = 25;

        if (!inWarp || isChargingWarp)
        {
            newSize = normalCamSizeCurve.Evaluate(currentSpeed / maxSpeed);
        }
        else
        {
            newSize = normalCamSizeCurve.Evaluate(currentSpeed / maxWarpSpeed);
        }

        followCam.m_Lens.OrthographicSize = newSize;
    }

    public void MoveAndTurn()
    {
        if (!inWarp && !isChargingWarp)
        {
            rb.AddForce(-transform.up * moveSpeed * inputY);          
        }       

        if (!inWarp && !isChargingWarp)
        {
            //Turning left and right
            if (rb.angularVelocity < 0)
            {
                if (rb.angularVelocity > -maxAngVel)
                {
                    rb.AddTorque(-inputX * rotationSpeed);
                }
            }
            else if (rb.angularVelocity > 0)
            {
                if (rb.angularVelocity < maxAngVel)
                {
                    rb.AddTorque(-inputX * rotationSpeed);
                }
            }
            else
            {
                rb.AddTorque(-inputX * rotationSpeed);
            }
        }

        #region Dampeners and drag
        if (!inWarp && !isChargingWarp)
        {           
            if (dampeners && inputY != 0)
            {
                rb.drag = dragCurve.Evaluate(currentSpeed / maxSpeed);
                rb.angularDrag = angularDrag;
                AddForceForSlowingDown();
            }
            else if(dampeners && inputY == 0)
            {
                rb.drag = dragCurve.Evaluate(1);
                rb.angularDrag = angularDrag;
            }
            else
            {
                rb.drag = 0;
                rb.angularDrag = 0;
            }
        }
        else
        {
            rb.drag = 0;
        }
        #endregion

        if(!inWarp && !isChargingWarp)
        {
            //Making the ship actually accelerate faster so it's not always stuck at a slow speed, but 
            // also limits the player from just jumping into their ship and escaping, as they start quite slow
            if (inputY > 0)
            {
                moveSpeed += accelAcceleration;
            }
            else
            {
                moveSpeed = baseMoveSpeed;
            }

            if (currentSpeed > maxSpeed)
            {
                rb.velocity = rb.velocity.normalized * maxSpeed;
            }
        }       
    }

    private void AddForceForSlowingDown()
    {
        float angleBetweenVelAndUp = Mathf.Atan2(rb.velocity.normalized.y - transform.up.y, rb.velocity.normalized.x - transform.up.x) * Mathf.Rad2Deg;
        rb.AddForce(-rb.velocity * 5 / (1 / Mathf.Abs(angleBetweenVelAndUp)));
        Debug.DrawLine(transform.position, -rb.velocity / (1 / Mathf.Abs(angleBetweenVelAndUp)), Color.magenta);
    }

    public void ChangeParticleThrustRate(int rate)
    {
        leftThrust.GetComponent<VisualEffect>().SetInt(Shader.PropertyToID("Spawn Rate"), rate);
        rightThrust.GetComponent<VisualEffect>().SetInt(Shader.PropertyToID("Spawn Rate"), rate);
    }

    public void ManageThrustVolume(float interval)
    {
        src.volume = thrusterSoundCurve.Evaluate(currentSpeed / maxSpeed);
    }

    public void FoldInWings()
    {
        rightWing.GetComponent<Animator>().SetTrigger("FoldIn");
        leftWing.GetComponent<Animator>().SetTrigger("FoldIn");
        wingsAudioSource.PlayOneShot(wingsFoldSound);
    }

    public void FoldOutWings()
    {
        rightWing.GetComponent<Animator>().SetTrigger("FoldOut");
        leftWing.GetComponent<Animator>().SetTrigger("FoldOut");
        wingsAudioSource.PlayOneShot(wingsFoldSound);
    }
}