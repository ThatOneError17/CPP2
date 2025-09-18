using System.Collections;
using UnityEngine;

public class PlatformSpawner : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private GameObject platformPrefab; // Reference to the platform prefab
    [SerializeField] private float spawnInterval = 12f; // Time interval between spawns
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private Transform[] pathNodes;  //Passes this to the node based moving platform script
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
            GameObject newPlatform = Instantiate(platformPrefab, point.position, prefabRotation); // Spawn the platform at the current position and rotation

            if (pathNodes.Length > 0)
            {
                NodeBasedMovingPlatform platformScript = newPlatform.GetComponent<NodeBasedMovingPlatform>();
                platformScript.InitializeNodes(pathNodes);
            }
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
