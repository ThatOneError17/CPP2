using UnityEngine;

public class PowerUpRandomSpawner : MonoBehaviour
{

    public GameObject[] PowerUpPreFabs;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPowerUps();
    }

    void spawnPowerUps()
    {

        int rand = Random.Range(0, PowerUpPreFabs.Length);
        Instantiate(PowerUpPreFabs[rand], transform.position, transform.rotation);

    }
}
