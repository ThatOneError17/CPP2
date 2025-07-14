using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    
    InputSystem_Actions input;
    CharacterController controller;

    Vector2 direction;
    Vector3 velocity;

    public float speed = 10f;
    public float jumpHeight = 5f;
    public float timeToJumpApex = 1.0f; // Time to reach the apex of the jump

    private float gravity; // Gravity value for the jump
    private float jumpVelocity; // Velocity when jumping

    bool jumpPressed = false; // Whether the jump button is pressed

    private void Awake()
    {
        input = new InputSystem_Actions();

    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        

        try
        {
            controller = GetComponent<CharacterController>();
            if (controller == null) //Check if the CharacterController component is attached to the GameObject
            {
                throw new UnassignedReferenceException("CharacterController component not found on this GameObject.");
            }
            
        }

        catch(UnassignedReferenceException e)  //e object stores information oin exception
        {
            Debug.LogError(e.Message); //Sends error message to console
            Application.Quit(); //Quits application if CharacterController is not found
        }

        finally
        {
            Debug.Log("Input callbacks set successfully.");
        }

        input.Player.SetCallbacks(this); //Allows us to bind inputs automatically

        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2); // Calculate gravity based on jump force and time to apex
        jumpVelocity = -(gravity * timeToJumpApex); // Calculate the initial jump velocity

    }


    void FixedUpdate()  //Runs at framerate of game
    {
        if (GameManager.endOfLevel)  
        {
            return;
        }
        
        velocity.x = direction.x * speed; //Set the x velocity based on the input direction and speed
        velocity.z = direction.y * speed; //Set the z velocity based on the input direction and speed

        if(!controller.isGrounded) velocity.y += gravity * Time.deltaTime; //Apply gravity if not grounded, is needed for CharacterController
        else velocity.y = CheckJump(); //Reset y velocity if grounded (Can't be 0)

        controller.Move(velocity * Time.fixedDeltaTime); //Move the character controller based on the velocity and fixed delta time

    }

    float CheckJump()
    {
        return jumpPressed ? jumpVelocity : -controller.skinWidth;
    }

    //Interface for inputs
    public void OnMove(InputAction.CallbackContext context)
    {
        if (context.performed) direction = context.ReadValue<Vector2>(); //On input performed, read the value of the input
        if (context.canceled) direction = Vector2.zero;    //On input canceled, set the direction to zero
    }

    public void OnLook(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnInteract(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) jumpPressed = true;
        if (context.canceled) jumpPressed = false;
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }
}
