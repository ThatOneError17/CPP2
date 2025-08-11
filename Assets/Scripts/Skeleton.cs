using UnityEngine;

public class Skeleton : MonoBehaviour
{

    //Components
    Rigidbody rb;
    Transform playerTransform;
    Animator anim;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 5f; //Speed of rotation towards the player
    [SerializeField] private bool isDead = false;
    [SerializeField] private GameObject itemDrop;

    [Header("Range")]
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;
    [SerializeField] private float detectionRange = 5f; //Range at which the skeleton can detect the player

    [Header("Health")]  
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange && !isDead && !GameManager.playerDead)
        {
            if (minBounds != null && maxBounds != null && IsPlayerWithinBounds())
                MoveTowardPlayer();

            else
                MoveTowardPlayer();
        }

        else
            anim.SetBool("isWalking", false);

        if (currentHealth <= 0)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("isDead"))
            {
                isDead = Dead();
                if (isDead)
                {
                    anim.SetTrigger("isDead");
                    rb.isKinematic = true;
                    GetComponent<CapsuleCollider>().enabled = false;
                    Destroy(gameObject, 0.5f); // Destroy the skeleton after 0.5 seconds
                    if (itemDrop != null)
                    {
                        GameObject droppedItem = Instantiate(itemDrop, transform.position + new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 270));

                    }
                }
            }
        }

    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.CompareTag("Kick"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("isKicked")) return;
            anim.SetTrigger("isKicked");
            currentHealth -= 1;
        }

        if (hit.gameObject.CompareTag("Weapon"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("isWeaponHit")) return;
            anim.SetTrigger("isWeaponHit");
            currentHealth -= 2;
        }

        if (hit.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("Projectile hit the skeleton");
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("isProjectileHit")) return;
            anim.SetTrigger("isProjectileHit");
            currentHealth -= 1;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
       
    }

    private bool IsPlayerWithinBounds()
    {
        Vector3 playerPos = playerTransform.position;
        return playerPos.x >= minBounds.x && playerPos.x <= maxBounds.x && playerPos.z >= minBounds.z && playerPos.z <= maxBounds.z;
    }

    void MoveTowardPlayer()
    {
        anim.SetBool("isWalking", true);
        Vector3 moveDirection = new Vector3(playerTransform.position.x - transform.position.x, 0, playerTransform.position.z - transform.position.z).normalized;
        Vector3 moveVelocity = moveDirection * moveSpeed;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        rb.linearVelocity = moveVelocity;
    }

    bool Dead()
    {
        return true;
    }



}
