using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSuitMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    public float moveSpeed = 5;
    public float dashSpeed = 5;
    public float dashTime = 1;

    //Get the overarching player object.
    private GameObject playerObj;

    //Input variables
    private float inputX;
    private float inputY;
    //Movement variables.
    private Coroutine isDashing;
    private bool isOverShip = true;

    //Ref to the player's rigid body component.
    private Rigidbody2D rb;

    //Called at start of runtime.
    private void Start()
    {
        //Get the player character's rigid body component.
        rb = GetComponent<Rigidbody2D>();
        //Get the overarching player object.
        playerObj = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame - updates input and applies vitals.
    void Update()
    {
        #region Input 
        //Capture the player 'walk' inputs which will be applied in the fixed update.
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        //Rotate the player to the mouse cursor.
        RotatePlayerToMouse();

        //Make the player 'dash'.
        Dash();

        if (isOverShip == true && Input.GetKeyDown(KeyCode.F)) //Check if the player is trying to reenter ship.
        {
            playerObj.GetComponent<PlayersuitManager>().PlayerEnterCockpit(); //Player renters their ship.
        }
        #endregion
    }

    //Called every n of a second. Use for physics updating.
    private void FixedUpdate()
    {
        //Moves the player by adding velocity to the rigid body from the captured input.
        MovePlayer();
    }

    #region Movement Functions

    private void RotatePlayerToMouse()
    {
        //Get the mouse cursor's position.
        Vector3 mousePos = Input.mousePosition;
        //Set the z position.
        mousePos.z = 5.23f;

        //Gets the player's position - which is world space - in screen space.
        Vector3 objectPos = Camera.main.WorldToScreenPoint(transform.position);

        //Calcualte the difference between the mouse and player screen space positions.
        mousePos.x = mousePos.x - objectPos.x;
        mousePos.y = mousePos.y - objectPos.y;

        //Calcuate the angle the player needs to be rotated to.
        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        //Apply the calculated angle to the player.
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
    }

    #region Dashing

    private void Dash()
    {
        //Check for Dash input and that the player is not already dashing.
        if (Input.GetKeyDown(KeyCode.LeftShift) && isDashing == null) 
        {
            //Starts the dashing co-routine.
            isDashing = StartCoroutine(DodgeCoroutine());
        }       
    }

    //Co-routine which executes the player dash.
    private IEnumerator DodgeCoroutine()
    {     
        //For the duration of a dashTime will move the player forwards at the end of each frame at the higher dashSpeed.
        for (float timer = 0; timer < dashTime; timer += Time.deltaTime)
        {
            var endOfFrame = new WaitForEndOfFrame();
            rb.MovePosition(transform.position + (new Vector3(rb.velocity.normalized.x * dashSpeed * Time.deltaTime, rb.velocity.normalized.y * dashSpeed * Time.deltaTime, 0)));
            yield return endOfFrame;
        }

        //The dash is completed. Set to null as the player is no longer dashing and can dash again.
        isDashing = null;
    }

    #endregion

    //Moves the player by adding velocity to the player's rigid body from the captured input.
    private void MovePlayer()
    {
        //Set the velocity of the rigid body.
        rb.velocity = new Vector2(inputX * moveSpeed, inputY * moveSpeed);
    }

    #endregion

    #region Collisions

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //Checking to see if the player is over the ship entry point.
        if (collision.CompareTag("PlayerSuitSpawner"))
        {
            isOverShip = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Checking to see if the player has left the ship entry point.
        if (collision.CompareTag("PlayerSuitSpawner"))
        {
            isOverShip = false;
        }
    }

    #endregion
}
