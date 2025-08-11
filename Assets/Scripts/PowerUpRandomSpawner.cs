using UnityEngine;

public class PowerUpRandomSpawner : MonoBehaviour
{

    public GameObject[] PowerUpPreFabs;
    [SerializeField] private int item;
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

        if (item == -1)
        {

            int rand = Random.Range(0, PowerUpPreFabs.Length - 1);
            if (rand != 3)
                Instantiate(PowerUpPreFabs[rand], transform.position, transform.rotation);
            else
                Instantiate(PowerUpPreFabs[item], transform.position + new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 270));
        }
        else
        {
            Instantiate(PowerUpPreFabs[item], transform.position, transform.rotation);
        }
    }
}
