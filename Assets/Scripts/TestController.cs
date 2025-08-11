using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.Animations;
using System.Collections;

public class TestController : MonoBehaviour, InputSystem_Actions.IPlayerActions
{
    //Components 
    InputSystem_Actions input;
    CharacterController controller;
    Camera mainCamera;
    Animator anim;

    //Movement variables
    Vector2 direction;
    Vector3 velocity;

    LayerMask enemyLayer;

    private float curSpeed = 5.0f;
    [Header("Movement Settings")]
    [SerializeField] private float initSpeed = 5.0f; //Initialspeed of the character
    [SerializeField] private float maxSpeed = 10.0f; //Speed of the character
    [SerializeField] private float moveAccel = 1f; //Acceleration for movement
    [SerializeField] private float rotationSpeed = 5.0f;
    


    //Jump variables
    [Header("Jump Settings")]
    [SerializeField] private float jumpHeight = 5f;
    [SerializeField] private float timeToJumpApex = 1.0f; //Time to reach the apex of the jump

    [Header("Weapon Settings")]
    [SerializeField] private Transform weaponAttachPoint; //The point where the weapon will be attached to the player
    [SerializeField] private GameObject kickHitBox; //The point where the kick hitbox will be attached to the player
    WeaponBase weapon = null; //Reference to the weapon attached to the player

    [Header("Health")]
    [SerializeField] private int maxHealth = 5; //Maximum health of the player
    [SerializeField] private int health = 5; //Maximum health of the player
    [SerializeField] private float invincibilityDuration = 1f; //Invincibility duration after taking damage
    public bool isInvincible = false; //Whether the player is invincible or not

    [Header("Miscellaneous")]
    [SerializeField] private float gravityPowerUpDuration = 10f; 
    [SerializeField] private float speedPowerUpDuration = 10f;
    [SerializeField] private float speedChange = 10f;
    [SerializeField] private float gravityChange;


    //Gravity and velocity
    private float gravity; // Gravity value for the jump
    private float jumpVelocity; // Velocity when jumping
    bool jumpPressed = false; // Whether the jump button is pressed

    bool canShoot = true; // Whether the player can shoot projectiles

    //For shooting projectiles
    private Shoot shoot;

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
        shoot = GetComponent<Shoot>();
        controller = GetComponent<CharacterController>();
        anim = GetComponentInChildren<Animator>();

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
        if (GameManager.endOfLevel || GameManager.gameOver)
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

        if (health <= 0) //Check if health is less than or equal to 0
        {
            Death(); //Call the Death function if health is 0
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
        if (direction == Vector2.zero) curSpeed = initSpeed; //If no input, set speed to initial speed
        else curSpeed = Mathf.MoveTowards(curSpeed, maxSpeed, moveAccel * Time.fixedDeltaTime); //If input, increase speed towards max speed

        velocity.x = projectedMoveDirection.x * curSpeed; //Set the x velocity based on the input direction and speed
        velocity.z = projectedMoveDirection.z * curSpeed; //Set the z velocity based on the input direction and speed

        if (!controller.isGrounded) velocity.y += gravity * Time.deltaTime; //Apply gravity if not grounded, is needed for CharacterController
        else velocity.y = CheckJump(); //Reset y velocity if grounded (Can't be 0)

    }

    void Update()
    {
        float speedRatio = curSpeed / maxSpeed; //Calculate speed ratio for animation
        if (direction == Vector2.zero) speedRatio = 0.0f; //If no input, set speed ratio to 0
        anim.SetFloat("curSpeed", speedRatio); //Set the speed parameter in the animator

        Ray newRay = new Ray(transform.position, transform.forward);
        RaycastHit hitInfo;
        Debug.DrawLine(transform.position, transform.position + (transform.forward * 10.0f), Color.red);

        if (Physics.Raycast(newRay, out hitInfo, 10.0f,  enemyLayer))
        {
            Debug.Log(hitInfo.transform.gameObject.name);
        }

        if (!controller.isGrounded)
        {
            transform.SetParent(null);
        }
    }

    public void Death() //Function to handle player death
    {
        Debug.Log("Player has died");
        anim.SetTrigger("isDead"); //Trigger the death animation
        GameManager.playerDead = true; //Set player dead to true
        GameManager.gameOver = true; //Set end of level to true
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
        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Attack01")) return; //If Attack01 animation is already playing, do not trigger again

        if(anim.GetCurrentAnimatorStateInfo(0).IsName("Kick")) return; //If Kick animation is already playing, do not trigger again

        if (weapon) anim.SetTrigger("Attack"); //If weapon is equipped, trigger the attack animation

        else
        {
            StartKick(); //If no weapon is equipped, trigger the kick animation
        }
    }


    public void OnInteract(InputAction.CallbackContext context)
    {
        if(weapon)
        {
            //weapon.Drop(controller); //Drop the weapon if it is equipped   Need to refer to Hisham's script for this
            weapon = null; //Set the weapon reference to null after dropping
        }


    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (canShoot)
        {
            shoot.Fire(); //Call the Fire function from the Shoot script when crouch is pressed
            canShoot = false; //Set canShoot to false to prevent firing again immediately
            StartCoroutine(HandleProjectileFireRate()); //Start the coroutine to handle the fire rate
        }
        }

    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.started) jumpPressed = true;
        if (context.canceled) jumpPressed = false;
    }

    public void OnPrevious(InputAction.CallbackContext context)
    {
        if (anim.GetCurrentAnimatorStateInfo(0).IsName("Wave")) 
            return;
        anim.SetTrigger("Wave"); 
    }

    public void OnNext(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        //throw new System.NotImplementedException();
    }

    private void OnControllerColliderHit(ControllerColliderHit hit) //Collision detection for things
    {
        if (hit.gameObject.CompareTag("Enemy"))
        {
            if(GameManager.endOfLevel) return;
            
            
            if (!isInvincible) //Check if the player is not invincible
            {
                Debug.Log("Collided with enemy and about to lose health");
                loseHealth(); //Call the loseHealth function to reduce health
                Debug.Log("Health: " + health); //Log the current health to the console
            }     
        }


        if (hit.collider.CompareTag("Weapon") && weapon == null)
        {
            anim.SetBool("hasWeapon", true); //Set the hasWeapon parameter in the animator to true  
            weapon = hit.collider.GetComponent<WeaponBase>(); //Get the WeaponBase component from the collided weapon
            weapon.Equip(controller, weaponAttachPoint); //Equip the weapon to the player

        }

        if (hit.collider.CompareTag("KeyDoor"))
        {
            if (GameManager.hasKey) //Check if the player has a key
            {
                Destroy(hit.collider.gameObject); //Destroy the door if the player has a key
                GameManager.hasKey = false; //Remove the key from the player
                Debug.Log("Door opened, key used");
            }
            else
            {
                Debug.Log("You need a key to open this door!"); //Log message if player does not have a key
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Enemy"))
        {
            if (GameManager.endOfLevel) return;
            
            Debug.Log("Collided with enemy");

            if (!isInvincible) //Check if the player is not invincible
            {
                loseHealth(); //Call the loseHealth function to reduce health
                StartCoroutine(HandleInvincibilityFrames());
                Debug.Log("Health: " + health); //Log the current health to the console
            }
        }

        if (other.CompareTag("EnemyWeapon"))
        {
            if (GameManager.endOfLevel) return;

            Debug.Log("Collided with enemy weapon");

            if (!isInvincible) //Check if the player is not invincible
            {
                loseTwoHealth(); //Call the loseTwoHealth function to reduce health
                StartCoroutine(HandleInvincibilityFrames());
                Debug.Log("Health: " + health); //Log the current health to the console
            }
        }

    }

    public void StartKick()
    {
        anim.SetTrigger("Kick"); 
        StartCoroutine(HandleKickHitbox()); //Start the coroutine to handle the kick hitbox
    }

    public void loseHealth()
    {
        StartCoroutine(HandleInvincibilityFrames());
        health--; //Reduce health by 1 when called
    }

    public void loseTwoHealth()
    {
        StartCoroutine(HandleInvincibilityFrames());
        health -= 2; //Reduce health by 2 when called
        if (health < 0) health = 0; //Ensure health does not go below 0
    }

    IEnumerator HandleKickHitbox()
    {
        yield return new WaitForSeconds(0.1f); //Delay before hitbox becomes active 
        kickHitBox.SetActive(true);
        yield return new WaitForSeconds(1f); //Duration of active hitbox
        kickHitBox.SetActive(false);
    }

    IEnumerator HandleProjectileFireRate()
    {
        yield return new WaitForSeconds(1f); //Delay before the next projectile can be fired
        canShoot = true; //Allow firing again after the delay
    }

    public IEnumerator HandleInvincibilityFrames()
    {
        isInvincible = true; //Set invincibility to true when health is lost
        yield return new WaitForSeconds(invincibilityDuration);
        isInvincible = false; //Set invincibility to false after the duration
    }

    public int Heal()
    {
        if (health < maxHealth) //Check if health is less than maximum health
        {
            health++; //Increase health by 1
            Debug.Log("Health increased to: " + health); //Log the new health to the console
        }
        else
        {
            Debug.Log("Health is already at maximum: " + maxHealth); //Log if health is already at maximum
        }
        return health; //Return the current health after healing
    }

    public void lowGravity()
    {
        gravity += gravityChange;
        StartCoroutine(ResetGravityAfterDelay());

    }

    public void speedBoost()
    {
        moveAccel += speedChange; //Increase the current accel
        maxSpeed += speedChange; //Increase the max speed
        Debug.Log("Speed boosted to: " + curSpeed); //Log the new speed to the console
        StartCoroutine(ResetSpeedAfterDelay());
    }

    private IEnumerator ResetSpeedAfterDelay()
    {
        Debug.Log("Coroutine started, waiting " + speedPowerUpDuration + " seconds.");
        yield return new WaitForSeconds(speedPowerUpDuration);
        moveAccel -= speedChange; //Reset accel to normal value
        maxSpeed -= speedChange; //Reset max speed to normal value
        curSpeed = initSpeed; //Reset current speed to initial speed
        Debug.Log("Speed reset to normal.");
    }

    private IEnumerator ResetGravityAfterDelay()
    {
        yield return new WaitForSeconds(gravityPowerUpDuration);
        gravity = (-2 * jumpHeight) / Mathf.Pow(timeToJumpApex, 2); //Reset gravity to normal value
        Debug.Log("Gravity reset to normal.");
    }

    public int getHealth()
    {
        return health; //Return the current health of the player
    }

    public int getMaxHealth()
    {
        return maxHealth; //Return the maximum health of the player
    }



    //private void OnCollisionStay(collision collision)   //Collision detection for enemies
    //{
    //    if (hit.gameObject.CompareTag("Enemy"))
    //    {
    //        Debug.Log("Collided with enemy");
    //    }
    //}
}
