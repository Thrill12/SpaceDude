using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PlayerShipMovement : MonoBehaviour
{
    public bool isPlayerInShip = true;

    [Space(5)]

    public CinemachineVirtualCamera followCam;

    public float startFOV = 95;
    public float maxFOV = 120;

    public float moveSpeed;
    private float baseMoveSpeed;
    public float accelAcceleration;
    public float maxSpeed = 1000;
    public float maxAngVel = 100;
    public float drag = 5;
    public float angularDrag = 5;

    [Space(5)]
    public float rotationSpeed;

    private Rigidbody2D rb;

    private Vector2 posOfMouseOnWorld;
   
    public bool dampeners = true;
    private float inputX;
    private float inputY;
    private Vector2 mousePos;
    [HideInInspector]
    public float currentSpeed;
    private PlayersuitManager playerSuit;

    public GameObject thrust;

    private AudioSource src;
    private float lastVol = 0;
    private float currentVol = 0;
    [HideInInspector]
    public UIManager ui;

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

        if (!isPlayerInShip) return;

        if (Input.GetMouseButton(0))
        {
            posOfMouseOnWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.F))
        {
            playerSuit.PlayerLeaveShip();
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dampeners = !dampeners;
        }

        if (Input.GetKeyDown(KeyCode.F1))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        ManageThrustVolume(0.001f);
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
        float newFOV = rb.velocity.magnitude * 2;

        if(newFOV > startFOV)
        {
            if (newFOV < maxFOV)
            {
                followCam.m_Lens.FieldOfView = newFOV;
            }
        }
        else
        {
            followCam.m_Lens.FieldOfView = startFOV;
        }        
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

        currentSpeed = rb.velocity.magnitude;

        if (inputY > 0)
        {
            moveSpeed += accelAcceleration;
        }
        else
        {
            moveSpeed = baseMoveSpeed;
        }

        if (inputY != 0)
        {
            thrust.SetActive(true);
        }
        else
        {
            thrust.SetActive(false);
        }

        if (currentSpeed > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }        
    }

    public void ManageThrustVolume(float interval)
    {
        if (rb.velocity.magnitude >= 0.5f)
        {
            if (currentVol < 1)
            {
                currentVol += interval;
            }
            else
            {
                currentVol = 1;
            }

            if (currentVol > lastVol)
            {
                //Increasing
                lastVol = currentVol;
            }
            else if (currentVol < lastVol)
            {
                //Decreasing
                lastVol = currentVol;
            }
            else
            {
                //Same
                lastVol = currentVol;
            }
        }
        else
        {
            currentVol = 0;
        }

        src.volume = currentVol;
    }
}