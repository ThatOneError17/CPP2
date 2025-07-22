using UnityEngine;

public class Enemy : MonoBehaviour
{
    //Components
    Rigidbody rb;
    Transform playerTransform;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {

        Vector3 moveDirection = new Vector3(playerTransform.position.x - transform.position.x, 0, playerTransform.position.z - transform.position.z).normalized;
        Vector3 moveVel = moveDirection * 5f; // Speed of the enemy
        moveVel.y = rb.linearVelocity.y;

        rb.linearVelocity = moveVel;
    }
}
