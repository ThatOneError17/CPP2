using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class KnightNPC : MonoBehaviour
{
    public enum EnemyState
    {
        Chase,
        Patrol
    }

    //Components
    //Rigidbody rb;
    Transform playerTransform;
    Animator anim;
    NavMeshAgent navAgent;
    Transform target;
    CapsuleCollider cc;

    //[Header("Movement")]
    //[SerializeField] private float moveSpeed = 5f;
    //[SerializeField] private float rotationSpeed = 5f; //Speed of rotation towards the player

    [Header("Misc.")]
    [SerializeField] private bool isDead = false;
    [SerializeField] private GameObject itemDrop;
    [SerializeField] private GameObject swordHitBox;

    [Header("Range")]
    [SerializeField] private float detectionRange = 5f; //Range at which the skeleton can detect the player

    [Header("Health")]
    [SerializeField] private int maxHealth = 5;
    private int currentHealth;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public EnemyState currentState;
    public Transform[] patrolPoints;
    public int patrolIndex = 0;
    public float distanceThreshold = 0.2f;
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cc = GetComponent<CapsuleCollider>();
        anim = GetComponentInChildren<Animator>();
        navAgent = GetComponent<NavMeshAgent>();
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (currentHealth < maxHealth)  //Assumes the player has hit the NPC and therefore will react
        {
            if (!playerTransform || GameManager.playerDead)
            {
                anim.SetBool("isCharging", false);
                currentState = EnemyState.Patrol;
                target = patrolPoints[patrolIndex];
            }
            if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange && !isDead && !GameManager.playerDead)
            {
                currentState = EnemyState.Chase;
                target = playerTransform;

                if (currentState == EnemyState.Chase)
                {
                    anim.SetBool("isWalking", false);
                    anim.SetBool("isCharging", true);
                    navAgent.speed = 4.5f; //Speed when chasing the player
                    navAgent.angularSpeed = 600f; //Angular speed for turning towards the player

                    float angleToPlayer = Vector3.Angle(transform.forward, playerTransform.position - transform.position);

                    if (navAgent.remainingDistance <= navAgent.stoppingDistance && angleToPlayer < 30f)
                    {
                        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
                        {
                            //Debug.Log("Knight is attacking");
                            Attack();
                        }

                    }
                }
            }

            else
            {
                currentState = EnemyState.Patrol;
                target = patrolPoints[patrolIndex];
            }

            if (currentState == EnemyState.Patrol)
            {
                navAgent.speed = 3.5f; //Speed when chasing the player
                navAgent.angularSpeed = 120f; //Angular speed for turning towards the patrol point
                anim.SetBool("isWalking", true);
                if (navAgent.remainingDistance <= distanceThreshold && !navAgent.pathPending)
                {
                    patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                    target = patrolPoints[patrolIndex];
                }
            }

        }

        else
        {
            anim.SetBool("isWalking", true);
            if (currentState == EnemyState.Patrol)
            {
                if (target == playerTransform)
                {
                    target = patrolPoints[patrolIndex];
                }
                if (navAgent.remainingDistance < distanceThreshold)
                {
                    patrolIndex = (patrolIndex + 1) % patrolPoints.Length; // Loop through patrol points
                    target = patrolPoints[patrolIndex];
                }
            }
        }
        navAgent.SetDestination(target.position);

        if (currentHealth <= 0)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Death"))
            {
                Dead();
            }
        }

    }

    //Weapon collision detection
    private void OnTriggerEnter(Collider hit)
    {
        Debug.Log("Hit detected on Knight");
        if (hit.gameObject.CompareTag("Kick"))
        {
            currentHealth -= 1;
        }

        if (hit.gameObject.CompareTag("Weapon"))
        {
            currentHealth -= 2;
        }

        if (hit.gameObject.CompareTag("Projectile"))
        {
            currentHealth -= 1;
        }
        Debug.Log("Current Health: " + currentHealth);
    }

    

    //void targetPosition()
    //{
    //    if (!playerTransform)
    //        return;

    //    if (currentState == EnemyState.Chase)
    //    {
    //        anim.SetBool("isCharging", true);
    //        target = playerTransform;
    //    }
    //    if (currentState == EnemyState.Patrol)
    //    {
    //        if (target == playerTransform)
    //        {
    //            target = patrolPoints[patrolIndex];
    //        }
    //        if (navAgent.remainingDistance < distanceThreshold)
    //        {
    //            patrolIndex = (patrolIndex + 1) % patrolPoints.Length; // Loop through patrol points
    //            target = patrolPoints[patrolIndex];
    //        }
    //    }
    //}

    //void MoveTowardsSpawn()
    //{
    //    anim.SetBool("isWalking", true);
    //    Vector3 moveDirection = new Vector3(startPos.x - transform.position.x, 0, startPos.z - transform.position.z).normalized;
    //    Vector3 moveVelocity = moveDirection * moveSpeed;
    //    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
    //    transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    //    //rb.linearVelocity = moveVelocity;
    //}

    private IEnumerator attackCooldown()
    {
        anim.SetTrigger("isAttacking");
        swordHitBox.SetActive(true); //Activate the sword hitbox
        yield return new WaitForSeconds(1f); //Cooldown until attack is finished
        swordHitBox.SetActive(false); //Deactivate the sword hitbox
    }

    void Attack()
    {
        StartCoroutine(attackCooldown());
    }    

    void Dead()
    {
        anim.SetTrigger("isDead");
        isDead = true;
        swordHitBox.SetActive(false);
        navAgent.isStopped = true; // Stop the NavMeshAgent
        cc.enabled = false; // Disable the capsule collider
        Destroy(gameObject, 2.5f); 
        if (itemDrop != null)
        {
            GameObject droppedItem = Instantiate(itemDrop, transform.position + new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 270));
            itemDrop = null;

        }
    }
}


