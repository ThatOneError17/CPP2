using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PowerUpRandomSpawner : MonoBehaviour
{

    public GameObject[] PowerUpPreFabs;
    [SerializeField] private int item;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private bool respawn = false;
    [SerializeField] private float respawnTime = 5f;
    private bool hasPowerUp = false; //Track if a power-up is currently present
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




    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PowerUp"))
        {
            hasPowerUp = true;
            Debug.Log("Power-up entered the spawner area.");

        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && respawn)
        {
            hasPowerUp = false;
            Debug.Log("Player exited the spawner area, starting respawn process.");
            StartCoroutine(RespawnPowerup());

        }
    }
 


    void spawnPowerUps()
    {

        if (item == -1)
        {

            int rand = Random.Range(0, PowerUpPreFabs.Length);
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

    private IEnumerator RespawnPowerup()
    {
        yield return new WaitForSeconds(respawnTime);
        spawnPowerUps();
    }
}


