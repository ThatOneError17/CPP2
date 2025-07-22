using UnityEngine;

public class PowerUp : MonoBehaviour // Inherit from MonoBehaviour to access Unity-specific methods and properties
{
    private float rotateSpeed = 60f; // Speed at which the power-up rotates
    public enum PickupType
    {
        Health,
        Boost,
        LowGravity,
    }

    public PickupType type;

    private void Update()
    {
        transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime); // Rotate the power-up around the Y-axis
    }

    private void OnTriggerEnter(Collider collision) 
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }

}
