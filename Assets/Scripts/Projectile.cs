using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

public class Projectile : MonoBehaviour
{

    [SerializeField] private ProjectileType type;
    [SerializeField] private float lifetime = 1f;

    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetVelocity(Vector2 velocity)
    {
        GetComponent<Rigidbody>().linearVelocity = velocity;
    }

    public void OnCollisionEnter(Collision collision)
    {
        if (type == ProjectileType.Enemy)
        {
            TestController player = collision.gameObject.GetComponent<TestController>();
            if (player != null)
            {
                Destroy(gameObject);
            }
        }
    }

    public enum ProjectileType
    {
        Enemy,
    }

}
