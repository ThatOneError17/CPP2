using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;

public class TestController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    //Components 
    InputSystem_Actions input;
    CharacterController controller;
    Camera mainCamera;

    //Movement variables
    Vector2 direction;
    Vector3 velocity;

    LayerMask enemyLayer;

    [Header("Movement Settings")]
    [SerializeField] private float speed = 10f;
    [SerializeField] private float rotationSpeed = 5.0f;

    //Jump variables
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float timeToJumpApex = 1.0f; // Time to reach the apex of the jump

    //Gravity and velocity
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

        catch (UnassignedReferenceException e)  //e object stores information oin exception
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

        mainCamera = Camera.main;
        enemyLayer = LayerMask.GetMask("Enemy");

    }



    void FixedUpdate()  //Runs at framerate of game
    {
        if (GameManager.endOfLevel)
        {
            return;
        }

        Vector3 projectedMoveDirection = ProjectedMoveDirection();
        UpdateCharacterVelocity(ProjectedMoveDirection());

        controller.Move(velocity * Time.fixedDeltaTime); //Move the character controller based on the velocity and fixed delta time

        //Rotate towards direction of movement
        if (direction != Vector2.zero)
        {
            float timeStep = rotationSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(projectedMoveDirection), timeStep);

        }

    }

    private Vector3 ProjectedMoveDirection()
    {
        Vector3 cameraFwd = mainCamera.transform.forward;
        Vector3 cameraRight = mainCamera.transform.right;

        cameraFwd.y = 0;
        cameraRight.y = 0;

        cameraFwd.Normalize();
        cameraRight.Normalize();

        return cameraFwd * direction.y + cameraRight * direction.x;
    }

    private void UpdateCharacterVelocity(Vector3 projectedMoveDirection)
    {
        velocity.x = projectedMoveDirection.x * speed; //Set the x velocity based on the input direction and speed
        velocity.z = projectedMoveDirection.z * speed; //Set the z velocity based on the input direction and speed

        if (!controller.isGrounded) velocity.y += gravity * Time.deltaTime; //Apply gravity if not grounded, is needed for CharacterController
        else velocity.y = CheckJump(); //Reset y velocity if grounded (Can't be 0)

    }

    void Update()
    {
        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        Debug.DrawLine(transform.position, transform.position + (transform.forward * 10.0f), Color.red);

        if (Physics.Raycast(newRay, out hitInfo, 10.0f,  enemyLayer))
        {
            Debug.Log(hitInfo.transform.gameObject.name);
        }
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

    private void OnControllerColliderHit(ControllerColliderHit hit) //Collision detection for enemies
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Collided with enemy");
            Destroy(gameObject);
            GameManager.endOfLevel = true;

        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("Collided with enemy");
            Destroy(gameObject);
            GameManager.endOfLevel = true;
        }
    }

    //private void OnCollisionStay(collision collision)   //Collision detection for enemies
    //{
    //    if (hit.gameObject.CompareTag("Enemy"))
    //    {
    //        Debug.Log("Collided with enemy");
    //    }
    //}
}
