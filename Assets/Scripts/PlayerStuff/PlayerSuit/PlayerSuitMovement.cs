using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSuitMovement : MonoBehaviour
{
    public float moveSpeed = 5;
    public float dashSpeed = 5;

    public GameObject playerFlashlight;

    private float inputX;
    private float inputY;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        RotatePlayerToMouse();
        ToggleFlashlight();
        Dash();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ToggleFlashlight()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            playerFlashlight.SetActive(!playerFlashlight.activeInHierarchy);
        }
    }

    private void RotatePlayerToMouse()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 5.23f;

        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    private void Dash()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            rb.AddForce(rb.velocity * dashSpeed, ForceMode2D.Impulse);
        }       
    }

    private void MovePlayer()
    {
        rb.velocity = new Vector2(inputX * moveSpeed, inputY * moveSpeed);
    }
}
