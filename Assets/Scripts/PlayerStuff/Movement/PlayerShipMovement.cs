using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PlayerMovement : MonoBehaviour
{
    public CinemachineVirtualCamera followCam;
    public Camera starCamera;

    public float minCamSize = 20;

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

    public GameObject thrust;

    private AudioSource src;
    private float lastVol = 0;
    private float currentVol = 0;
    [HideInInspector]
    public UIManager ui;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        baseMoveSpeed = moveSpeed;
        src = GetComponent<AudioSource>();
        ui = GameObject.FindGameObjectWithTag("UIManager").GetComponent<UIManager>();
    }

    private void Update()
    {
        if (ui.isInUI) return;

        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetMouseButton(0))
        {
            posOfMouseOnWorld = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetKeyDown(KeyCode.Z))
        {
            dampeners = !dampeners;
        }

        for (int i = 1; i < 10; i++)
        {
            if (Input.GetButtonDown(i.ToString()))
            {
                Debug.Log("Button " + i + " was pressed.");
            }
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
        MoveAndTurn();
    }

    public void ChangeCameraZoomVelocity()
    {
        if(rb.velocity.magnitude / 2 > minCamSize)
        {
            followCam.m_Lens.OrthographicSize = rb.velocity.magnitude / 2;
        }
        else
        {
            followCam.m_Lens.OrthographicSize = minCamSize;
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

        if (Input.GetAxis("Vertical") > 0)
        {
            moveSpeed += accelAcceleration;
            thrust.SetActive(true);
        }
        else
        {
            moveSpeed = baseMoveSpeed;
            thrust.SetActive(false);
        }

        if (currentSpeed > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }

        
    }

    public void ManageThrustVolume(float interval)
    {
        if (inputY != 0)
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
            if (currentVol > 0)
            {
                currentVol -= interval * 4;
            }
            else
            {
                currentVol = 0;
            }
        }

        src.volume = currentVol;
    }
}
