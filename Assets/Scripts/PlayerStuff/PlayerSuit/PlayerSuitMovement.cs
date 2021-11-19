using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSuitMovement : MonoBehaviour
{
    public float moveSpeed = 5;
    public float dashSpeed = 5;
    public float dashTime = 1;

    public GameObject playerFlashlight;

    private GameObject playerShip;
    private float inputX;
    private float inputY;
    private Coroutine isDashing;
    private bool isOverShip = true;

    private Rigidbody2D rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        playerShip = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        RotatePlayerToMouse();
        ToggleFlashlight();
        Dash();

        if(isOverShip && Input.GetKeyDown(KeyCode.F))
        {
            playerShip.GetComponent<PlayersuitManager>().PlayerEnterShip();
        }
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
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashing == null)
        {
            isDashing = StartCoroutine(DodgeCoroutine());
        }       
    }

    private IEnumerator DodgeCoroutine()
    {     
        for (float timer = 0; timer < dashTime; timer += Time.deltaTime)
        {
            var endOfFrame = new WaitForEndOfFrame();
            rb.MovePosition(transform.position + (new Vector3(rb.velocity.normalized.x * dashSpeed * Time.deltaTime, rb.velocity.normalized.y * dashSpeed * Time.deltaTime, 0)));
            yield return endOfFrame;
        }
        isDashing = null;
    }

    private void MovePlayer()
    {
        rb.velocity = new Vector2(inputX * moveSpeed, inputY * moveSpeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            isOverShip = true;
            Debug.Log("Is over ship");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.transform.CompareTag("Player"))
        {
            isOverShip = false;
            Debug.Log("Is not over ship");
        }
    }
}
