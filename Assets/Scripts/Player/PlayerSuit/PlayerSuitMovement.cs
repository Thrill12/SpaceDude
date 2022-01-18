using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSuitMovement : MonoBehaviour
{

    [Header("Movement Settings")]
    public float moveSpeed = 5;
    public float dashSpeed = 5;
    public float dashTime = 1;

    [Tooltip("Invisible object in scene to take the item in front of the player to interact with them.")]
    public GameObject frontChecker;

    //Get the overarching player object.
    private GameObject playerObj;
    public PlayerInput playerInput;

    //Input variables
    private float inputX;
    private float inputY;
    private Vector2 rotationInput;
    private Quaternion lastRotation;
    private Vector2 mousePosition;
    //Movement variables.
    private Coroutine isDashing;
    private bool isOverCockpit = true;
    private bool isOverExit = false;
    private bool isOverEntrance = false;

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
        lastRotation = transform.rotation;
    }

    //Called every n of a second. Use for physics updating.
    private void FixedUpdate()
    {
        //Moves the player by adding velocity to the rigid body from the captured input.
        MovePlayer();
    }

    public void InteractWithShipEntrance()
    {
        if (isOverCockpit == true) //Check if the player is trying to reenter ship.
        {
            rb.velocity = Vector2.zero;
            playerObj.GetComponent<PlayersuitManager>().PlayerEnterCockpit(); //Player renters their ship.
        }

        if (isOverExit == true) //Check if the player is trying to reenter ship.
        {
            rb.velocity = Vector2.zero;
            playerObj.GetComponent<PlayersuitManager>().PlayerExitShip(); //Player renters their ship.
        }

        if (isOverEntrance == true) //Check if the player is trying to reenter ship.
        {
            rb.velocity = Vector2.zero;
            playerObj.GetComponent<PlayersuitManager>().PlayerEnterShip(); //Player renters their ship.
        }
    }

    public void Interact(InputAction.CallbackContext context)
    {
        if(context.phase == InputActionPhase.Started)
        {
            FindInteract();
        }
    }

    private void FindInteract()
    {
        Debug.Log("Finding interactable");
        List<GameObject> nearby = Physics2D.OverlapCircleAll(frontChecker.transform.position, 0.2f).Select(x => x.gameObject).Where(x => x.GetComponent<Interactable>() || x.GetComponent<ItemHolder>()).ToList();
        if (nearby.Any())
        {
            if (nearby[0].GetComponent<ItemHolder>())
            {
                Debug.Log("Item");
                PickUpItem(nearby[0]);
                return;
            }
            else if (nearby[0].GetComponent<Interactable>())
            {
                Debug.Log("Interactable");
                nearby[0].GetComponent<Interactable>().Interact();
                return;
            }
        }

        InteractWithShipEntrance();
    }

    private void PickUpItem(GameObject item)
    {
        if (Inventory.instance.AddItem(item.GetComponent<ItemHolder>().itemHeld))
        {
            item.GetComponent<ItemHolder>().ClickedOn(gameObject);
        }
        else
        {
            Debug.Log("Inventory full, couldn't pick up");
        }
    }

    #region Movement Functions

    public void HandleTurning(InputAction.CallbackContext context)
    {
        if (playerInput.currentControlScheme == "GamePad")
        {
            RotatePlayerToGamepadStick(context);
        }
        else
        {
            RotatePlayerToMouse(context);
        }
    }

    private void RotatePlayerToMouse(InputAction.CallbackContext context)
    {    
        if(context.phase == InputActionPhase.Performed)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(context.ReadValue<Vector2>());
            //Gets the player's position - which is world space - in screen space.
            Vector3 objectPos = transform.position;

            Vector3 direction = new Vector3();

            //Calculate the difference between the mouse and player screen space positions.
            direction.x = mousePos.x - objectPos.x;
            direction.y = mousePos.y - objectPos.y;

            //Calculate the angle the player needs to be rotated to.
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            //Apply the calculated angle to the player.
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
        }
        
    }

    private void RotatePlayerToGamepadStick(InputAction.CallbackContext context)
    {
        if(playerInput.currentControlScheme == "GamePad")
        {
            if (context.phase == InputActionPhase.Performed)
            {
                rotationInput = context.ReadValue<Vector2>();

                //Calcuate the angle the player needs to be rotated to.
                float angle = Mathf.Atan2(rotationInput.x, rotationInput.y) * Mathf.Rad2Deg;

                if(angle != 0)
                {
                    transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle));
                }             
            }
            else
            {
                transform.rotation = lastRotation;
            }
        }       
    }

    public void ChangeMovementInputs(InputAction.CallbackContext context)
    {
        //Reads the input if the button pressed is held
        if (context.phase == InputActionPhase.Performed)
        {
            //Separates value given by the input in x and y components
            inputX = context.ReadValue<Vector2>().x;
            inputY = context.ReadValue<Vector2>().y;
        }
        else
        {
            inputX = 0;
            inputY = 0;
        }
    }

    #region Dashing

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started)
        {
            //Check for Dash input and that the player is not already dashing.
            if (isDashing == null)
            {
                //Starts the dashing co-routine.
                isDashing = StartCoroutine(DodgeCoroutine());
            }
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
            isOverCockpit = true;
        }

        if (collision.CompareTag("Ship Exit"))
        {
            isOverExit = true;
        }
        if (collision.CompareTag("Ship Entrance"))
        {
            isOverEntrance = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //Checking to see if the player has left the ship entry point.
        if (collision.CompareTag("PlayerSuitSpawner"))
        {
            isOverCockpit = false;
        }

        if (collision.CompareTag("Ship Exit"))
        {
            isOverExit = false;
        }
        if (collision.CompareTag("Ship Entrance"))
        {
            isOverEntrance = false;
        }
    }

    #endregion
}
