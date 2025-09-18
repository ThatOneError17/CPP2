using UnityEngine;

public class PowerUp : MonoBehaviour // Inherit from MonoBehaviour to access Unity-specific methods and properties
{
    private float rotateSpeed = 60f; // Speed at which the power-up rotates
    public enum PickupType
    {
        Health,
        Boost,
        LowGravity,
        Key,
    }

    public PickupType type;
    public AudioClip pickupSound; // Sound to play when the power-up is collected

    private void Update()
    {
        //if (PickupType.Health == type)
        //    transform.Rotate(Vector3.right * rotateSpeed * Time.deltaTime); // Rotate the power-up around the Y-axis

        if (PickupType.Key == type)
            transform.Rotate(Vector3.left * rotateSpeed * Time.deltaTime);

        else
            transform.Rotate(Vector3.up * rotateSpeed * Time.deltaTime);

    }

    private void OnTriggerEnter(Collider collision) 
    {
        if (collision.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(pickupSound, transform.position); //Play the pickup sound at the power-up's position
            if (type == PickupType.Health)
            {
                collision.GetComponent<TestController>().Heal(); // Heal the player by 20 health points
                Debug.Log("Player healed by 1 health points");
            }
            else if (type == PickupType.Boost)
            {
                collision.GetComponent<TestController>().speedBoost();
                Debug.Log("Player has speed boost");
            }
            else if (type == PickupType.LowGravity)
            {
                collision.GetComponent<TestController>().lowGravity(); // Apply low gravity effect to the player
                Debug.Log("Player has low gravity");
            }
            else if (type == PickupType.Key)
            {
                GameManager.hasKey = true; // Increment the player's key count
                Debug.Log("Player has a key");
            }
            Destroy(gameObject);
        }
    }

}
