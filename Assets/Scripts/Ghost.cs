using UnityEngine;

public class Ghost : MonoBehaviour
{

    //Components
    Rigidbody rb;
    Transform playerTransform;
    CapsuleCollider cc;
    private Shoot shoot;

    private bool isSeen = false;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float detectionDistance = 20f; //Max distance for line of sight check
    [SerializeField] private float visionThreshold = 0.5f; //Dot product threshold to count as "being looked at"
    [SerializeField] private float rotationSpeed = 5f; //Speed of rotation towards the player
    

    [Header("Range")]
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;
    [SerializeField] private float detectionRange = 5f; //Range at which the ghost can detect the player

    //Model Stuff
    [Header("Model Settings")]
    [SerializeField] private GameObject ghostModel; //Reference to the ghost model (if needed for future use)
    [SerializeField] private SkinnedMeshRenderer ghostModelAlt;
    [SerializeField] private float dissolveDuration = 2f;
    private Material ghostMaterial;
    private bool isDissolving = false;
    private float dissolveAmount = 0f;

    [SerializeField] private float projectileFireRate = 4f;
    private float timeSinceLastFire = 0;

   

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        shoot = GetComponent<Shoot>();
        cc = GetComponent<CapsuleCollider>();

        //ghostMaterial = ghostModelAlt.material;
        //ghostMaterial.SetFloat("Dissolve", 0f);

    }

    // Update is called once per frame
    void Update()
    {
        CheckIfSeen();

        //if (isDissolving && dissolveAmount < 1f)
        //{
        //    Debug.Log("Ghost is dissolving. Dissolve amount: " + dissolveAmount);
        //    dissolveAmount += Time.deltaTime / dissolveDuration;
        //    ghostMaterial.SetFloat("Dissolve", dissolveAmount);

        //    if (dissolveAmount >= 1f)
        //    {
        //        isDissolving = false;
        //    }
        //}

        if (!isSeen)
        {
            if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange && IsPlayerWithinBounds() && !GameManager.playerDead)   
            {
                MoveTowardPlayer();
                if (Time.time >= timeSinceLastFire + projectileFireRate)
                {
                    timeSinceLastFire = Time.time;
                    Debug.Log("Firing projectile");
                    shoot.Fire(); //Fire a projectile
                }
                
            }
            ghostModel.SetActive(true); //Makes Ghost visible when not seen

        }
        else
        {
            StopMoving();
            //isDissolving = true;
            //Debug.Log("Dissolve triggered!");
            ghostModel.SetActive(false); //Hide the ghost model when seen
        }


    }

    void CheckIfSeen()
    {
        Vector3 directionToGhost = (transform.position - playerTransform.position).normalized;

        Ray ray = new Ray(playerTransform.position, directionToGhost);
        RaycastHit hit;

        // Reset seen state
        isSeen = false;

        if (Physics.Raycast(ray, out hit, detectionDistance))
        {
            if (hit.transform == transform)
            {
                float dot = Vector3.Dot(playerTransform.forward, directionToGhost);
                if (dot > visionThreshold)
                {
                    isSeen = true;
                }
            }
        }
    }

    private bool IsPlayerWithinBounds()
    {
        Vector3 playerPos = playerTransform.position;
        return playerPos.x >= minBounds.x && playerPos.x <= maxBounds.x && playerPos.z >= minBounds.z && playerPos.z <= maxBounds.z;
    }

    void MoveTowardPlayer()
    {
        Vector3 moveDirection = new Vector3(playerTransform.position.x - transform.position.x, 0, playerTransform.position.z - transform.position.z).normalized;
        Vector3 moveVelocity = moveDirection * moveSpeed;

        Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);

        rb.linearVelocity = moveVelocity;
    }

    void StopMoving()
    {
        rb.linearVelocity = new Vector3(0, rb.linearVelocity.y, 0); //Only stop horizontal movement
    }


    
}

