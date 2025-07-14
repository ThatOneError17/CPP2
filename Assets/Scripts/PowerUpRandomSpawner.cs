using UnityEngine;

public class PowerUpRandomSpawner : MonoBehaviour
{

    public GameObject[] PowerUpPreFabs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        try
        {
            spawnPowerUps();
        }
        // Catch any exceptions that may occur during the spawning process
        catch (System.Exception e)
        {
            Debug.LogError("Error spawning power-ups: " + e.Message);
        }

        finally
        {
            Debug.Log("Power-up spawning process completed.");
        }
    }

    void spawnPowerUps()
    {

        int rand = Random.Range(0, PowerUpPreFabs.Length);
        Instantiate(PowerUpPreFabs[rand], transform.position, transform.rotation);

    }
}
