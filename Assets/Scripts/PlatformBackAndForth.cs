using UnityEngine;

public class PlatformBackAndForth : MonoBehaviour
{
    //Intended to only move in 1 direction at a time (X, Y, or Z), would need to revise to allow for multiple directions
    [Header("Platform Movement")]
    [SerializeField] private float speed = 2f; // Speed of the platform movement
    [SerializeField] private float maxXOffset = 0f; // Maximum distance the platform can move in the X direction
    [SerializeField] private float maxYOffset = 0f; // Maximum distance the platform can move in the Y direction
    [SerializeField] private float maxZOffset = 0f; // Maximum distance the platform can move in the Z direction
    

    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 destination;
    private bool movingToTarget = true;

    private Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        targetPosition = startPosition + new Vector3(maxXOffset, maxYOffset, maxZOffset);

        rb = GetComponent<Rigidbody>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (movingToTarget)
        {
            destination = targetPosition;
        }
        else
        {
            destination = startPosition;
        }
        rb.MovePosition(Vector3.MoveTowards(rb.position, destination, speed * Time.fixedDeltaTime));

        transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);

        if (Vector3.Distance(transform.position, destination) < 0.01f)
        {
            movingToTarget = !movingToTarget;
        }
    }
}
