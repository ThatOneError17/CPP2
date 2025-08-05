using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField] private GameObject platform; // Reference to the platform GameObject
    [SerializeField] private float speed = 5f; // Speed of the platform movement
    [SerializeField] private float destroyDelay = 12f; // Time after which the platform will be destroyed
    [SerializeField] private float x;
    [SerializeField] private float y;
    [SerializeField] private float z;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Destroy(platform, destroyDelay);
        Vector3 moveDirecrtion = new Vector3(x, y, z); // Define the movement direction (forward in this case)
        Vector3 moveVelocity = moveDirecrtion * speed;// Calculate the movement velocity based on speed and time
        moveVelocity.y = rb.linearVelocity.y;
        rb.linearVelocity = moveVelocity;
    }
}
