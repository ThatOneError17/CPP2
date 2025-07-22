using UnityEngine;
using UnityEngine.Audio;

public class Shoot : MonoBehaviour
{

    [SerializeField] private float projectileSpeed = 10f;
    [SerializeField] private Transform spawnPoint;

    [SerializeField] private Projectile projectilePrefab;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fire()
    {
        Projectile curProjectile = Instantiate(projectilePrefab, spawnPoint.position, Quaternion.identity);

        Vector3 shootDirection = transform.forward * projectileSpeed;
        curProjectile.SetVelocity(shootDirection);

    }
}
