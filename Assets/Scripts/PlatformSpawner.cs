using System.Collections;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject platformPrefab; // Reference to the platform prefab
    [SerializeField] private float spawnInterval = 12f; // Time interval between spawns
    [SerializeField] private Transform[] spawnPoints;
    void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SpawnAllPlatforms()
    {
        foreach (Transform point in spawnPoints)
        {
            Quaternion prefabRotation = platformPrefab.transform.rotation;
            Instantiate(platformPrefab, point.position, prefabRotation); // Spawn the platform at the current position and rotation
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            SpawnAllPlatforms();
            yield return new WaitForSeconds(spawnInterval);
        }
    }
}
