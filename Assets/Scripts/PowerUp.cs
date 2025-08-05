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
            if (type == PickupType.Health)
            {
                collision.GetComponent<TestController>().Heal(); // Heal the player by 20 health points
                Debug.Log("Player healed by 1 health points");
            }
            else if (type == PickupType.Boost)
            {
                //collision.GetComponent<TestController>().Boost(); // Apply a speed boost to the player
                Debug.Log("Player has speed boost");
            }
            else if (type == PickupType.LowGravity)
            {
                collision.GetComponent<TestController>().lowGravity(); // Apply low gravity effect to the player
                Debug.Log("Player has low gravity");
            }
            Destroy(gameObject);
        }
    }

}
