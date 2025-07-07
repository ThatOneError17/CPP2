using UnityEngine;

public class TestController : MonoBehaviour
{
    public int speed = 10;
    Rigidbody rb;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.endOfLevel)
        {
            return;
        }

        float hValue = Input.GetAxis("Horizontal");
        float vValue = Input.GetAxis("Vertical");

        Vector3 movement = (new Vector3(hValue * speed, 0, vValue).normalized * speed);
        movement.y = rb.linearVelocity.y; // Maintain the current vertical velocity
        rb.linearVelocity = movement;
    }
}
