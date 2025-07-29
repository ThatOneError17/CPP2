using UnityEngine;

public class Skeleton : MonoBehaviour
{

    //Components
    Rigidbody rb;
    Transform playerTransform;
    public Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        anim = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 moveDirection = new Vector3(playerTransform.position.x - transform.position.x, 0, playerTransform.position.z - transform.position.z).normalized; //Calculate the direction towards the player
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.CompareTag("Kick"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("isKicked")) return;
            anim.SetTrigger("isKicked");
        }

        if (hit.gameObject.CompareTag("Weapon"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("isWeaponHit")) return;
            anim.SetTrigger("isWeaponHit");
        }

        if (hit.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("Projectile hit the skeleton");
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("isProjectileHit")) return;
            anim.SetTrigger("isProjectileHit");
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
       
    }


}
