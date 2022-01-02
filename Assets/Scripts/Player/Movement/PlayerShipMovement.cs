using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.VFX;

public class PlayerShipMovement : MonoBehaviour
{
    public bool isPlayerInShip = true;

    [Space(5)]

    [Header("Camera Settings")]
    public CinemachineVirtualCamera followCam;
    public AnimationCurve camSizeCurve;

    [Header("Movement Settings")]
    public float moveSpeed;
    private float baseMoveSpeed;
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
    private float lastVol = 0;
    private float currentVol = 0;
    [HideInInspector]
    public UIManager ui;
    [HideInInspector]
    public int thrusterParticleSpawnRate = 0;

    private void Start()
    {
        isPlayerInShip = true;
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

        if (thrusterParticleSpawnRate > 5)
        {
            leftLight.intensity = 0.5f + (thrusterParticleSpawnRate / (maxSpeed * 250)) * 2;
            rightLight.intensity = 0.5f + (thrusterParticleSpawnRate / (maxSpeed * 250)) * 2;
        }
        else
        {
            leftLight.intensity = 0;
            rightLight.intensity = 0;
        }

        if (!isPlayerInShip)
        {
            inputY = 0;
            inputX = 0;
        }

        ManageThrustVolume(0.001f);

        if (!isPlayerInShip) return;

        thrusterParticleSpawnRate = (int)Mathf.Floor(currentSpeed * 250);

        if (Input.GetMouseButton(0))
        {
            posOfMouseOnWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.F))
        {
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

    private void FixedUpdate()
    {
        if (isPlayerInShip)
        {
            MoveAndTurn();
        }       
    }

    public void ChangeCameraZoomVelocity()
    {
        float newSize = camSizeCurve.Evaluate(rb.velocity.magnitude / 100);
        followCam.m_Lens.OrthographicSize = newSize;   
    }

    public void MoveAndTurn()
    {    
        rb.AddForce(-transform.up * moveSpeed * inputY);

        if(rb.angularVelocity < 0)
        {
            if (rb.angularVelocity > -maxAngVel)
            {
                rb.AddTorque(-inputX * rotationSpeed);
            }
        }
        else if(rb.angularVelocity > 0)
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

        if(inputX != 0)
        {
            rb.angularDrag = 0;
        }
        else
        {
            rb.angularDrag = angularDrag;
        }

        

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
    }

    public void FoldOutWings()
    {
        rightWing.GetComponent<Animator>().SetTrigger("FoldOut");
        leftWing.GetComponent<Animator>().SetTrigger("FoldOut");
    }
}