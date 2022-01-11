using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerShipMovement : MonoBehaviour
{
    public bool isPlayerPiloting = true;

    [Space(5)]

    [Header("Warp settings")]
    public Volume postProcessingVolume;
    public VisualEffect shipWarpEffectParticles;
    public float warpAcceleration = 1;
    public int maxWarpEffectRate = 500;
    public float maxWarpSpeed;
    public bool inWarp = false;
    public bool isChargingWarp = false;
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
    public float drag = 5;
    public float angularDrag = 5;
    public AnimationCurve thrusterSoundCurve;

    [Space(5)]
    public float rotationSpeed;

    [Space(10)]
    public GameObject rightWing;
    public GameObject leftWing;

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
    }

    private void Update()
    {
        if (ui.isInUI) return;

        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        ChangeParticleThrustRate(thrusterParticleSpawnRate);

        currentSpeed = rb.velocity.magnitude;

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
        }

        ManageThrustVolume(0.001f);

        if (!isPlayerPiloting) return;

        if (Input.GetKeyDown(KeyCode.G) && !inWarp && !isChargingWarp)
        {
            ActivateWarp();
        }        
        else if(Input.GetKeyDown(KeyCode.G) && inWarp && !isChargingWarp)
        {
            DropOutWarp();
        }

        thrusterParticleSpawnRate = (int)Mathf.Floor(currentSpeed * 250);

        if (Input.GetMouseButton(0))
        {
            posOfMouseOnWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.F))
        {
            //if (currentSpeed / maxSpeed * 100 < 20)
            playerSuit.PlayerLeaveCockpit();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dampeners = !dampeners;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        ChangeCameraZoomVelocity();
    }

    private void ActivateWarp()
    {
        rb.freezeRotation = true;
        shipWarpEffectParticles.SetInt(Shader.PropertyToID("Spawn Rate"), maxWarpEffectRate / 2);
        isChargingWarp = true;
        warpSource.PlayOneShot(warpInSound);
        Invoke("DropInWarp", 1.6f);
    }

    private void DropInWarp()
    {
        isChargingWarp = false;
        inWarp = true;
        warpCamera.Priority = 1000;

        foreach (var item in GetComponents<Collider2D>().Where(x => x.isTrigger != true))
        {
            item.enabled = false;
        }

        StartCoroutine(IncreaseSpeedInWarp());
        StartCoroutine(IncreaseBlurAmount());
        StartCoroutine(IncreaseWarpEffectParticleRate());     
    }

    private void DropOutWarp()
    {
        inWarp = false;
        warpCamera.Priority = 0;

        shipWarpEffectParticles.SetInt(Shader.PropertyToID("Spawn Rate"), 0);

        foreach (var item in GetComponents<Collider2D>().Where(x => x.isTrigger != true))
        {
            item.enabled = true;
        }

        MotionBlur blur = null;
        postProcessingVolume.profile.TryGet<MotionBlur>(out blur);
        blur.intensity.value = 0;

        warpSource.PlayOneShot(warpOutSound);
        rb.freezeRotation = false;
    }

    IEnumerator IncreaseWarpEffectParticleRate()
    {
        while(shipWarpEffectParticles.GetInt(Shader.PropertyToID("Spawn Rate")) < maxWarpEffectRate && inWarp)
        {
            shipWarpEffectParticles.SetInt(Shader.PropertyToID("Spawn Rate"), shipWarpEffectParticles.GetInt(Shader.PropertyToID("Spawn Rate")) + 1);

            yield return new WaitForSeconds(0.005f);
        }
    }

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
            rb.velocity = -transform.up * warpAcceleration * rb.velocity.magnitude;
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

        if (!inWarp && !isChargingWarp)
        {
            #region Dampeners and drag
            if (dampeners)
            {
                if (inputY != 0)
                {
                    rb.drag = 0;
                }
                else
                {
                    rb.drag = drag;
                }
            }
            else
            {
                rb.drag = 0;
            }


            if (inputX != 0)
            {
                rb.angularDrag = 0;
            }
            else
            {
                rb.angularDrag = angularDrag;
            }
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