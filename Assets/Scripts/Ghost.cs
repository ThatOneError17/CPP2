using UnityEngine;

public class Ghost : MonoBehaviour
{

    //Components
    Rigidbody rb;
    Transform playerTransform;
    private Shoot shoot;

    private bool isSeen = false;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float detectionDistance = 20f; //Max distance for line of sight check
    [SerializeField] private float visionThreshold = 0.5f; //Dot product threshold to count as "being looked at"
    [SerializeField] private float rotationSpeed = 5f; //Speed of rotation towards the player

    [SerializeField] private float projectileFireRate = 4f;
    private float timeSinceLastFire = 0;

    //Model stuff
    //private SkinnedMeshRenderer skinRenderer;
    //private Material ghostMaterial;
    //private Color originalColor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        shoot = GetComponent<Shoot>();
        //skinRenderer = GetComponentInChildren<SkinnedMeshRenderer>(); //Grab the SkinnedMeshRenderer from the ghost's mesh

        //if (skinRenderer != null)
        //{
        //    ghostMaterial = skinRenderer.material;
        //    originalColor = ghostMaterial.color;
        //}
        //else
        //{
        //    Debug.LogError("SkinnedMeshRenderer not found in children!");
        //}

        //ghostMaterial = GetComponentInChildren<MeshRenderer>().material; //Grab the material from the ghost's mesh renderer
        //originalColor = ghostMaterial.color;
    }

    // Update is called once per frame
    void Update()
    {
        CheckIfSeen();

        if (!isSeen)
        {
            MoveTowardPlayer();
            if (Time.time >= timeSinceLastFire + projectileFireRate)
            {
                timeSinceLastFire = Time.time;
                Debug.Log("Firing projectile");
                shoot.Fire(); //Fire a projectile
            }


        }
        else
        {
            StopMoving();
        }


    }

    void CheckIfSeen()
    {
        Vector3 directionToGhost = (transform.position - playerTransform.position).normalized;

        Ray ray = new Ray(playerTransform.position, directionToGhost);
        RaycastHit hit;

        // Reset seen state
        //SetOpacity(1f); //Make ghost fully opaque
        isSeen = false;

        if (Physics.Raycast(ray, out hit, detectionDistance))
        {
            if (hit.transform == transform)
            {
                float dot = Vector3.Dot(playerTransform.forward, directionToGhost);
                if (dot > visionThreshold)
                {
                    //SetOpacity(0.3f); //Make ghost semi-transparent
                    isSeen = true;
                }
            }
        }
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

    //void SetOpacity(float alpha)    //Used for changing the ghost's opacity
    //{
    //    Color newColor = originalColor;
    //    newColor.a = alpha;
    //    ghostMaterial.color = newColor;
    //}

    
}

