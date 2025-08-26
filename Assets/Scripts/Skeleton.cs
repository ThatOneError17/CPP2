using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class Skeleton : MonoBehaviour
{
    public enum EnemyState
    {
        Chase,
        Patrol
    }

    //Components
    Rigidbody rb;
    Transform playerTransform;
    Transform target;
    Animator anim;
    NavMeshAgent navAgent;

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private bool isDead = false;
    [SerializeField] private GameObject itemDrop;

    [Header("Range")]
    [SerializeField] private Vector3 minBounds;
    [SerializeField] private Vector3 maxBounds;
    [SerializeField] private float detectionRange = 5f; //Range at which the skeleton can detect the player

    [Header("Health")]  
    [SerializeField] private int maxHealth = 3;
    private int currentHealth;

    [Header("Navigation")]
    public EnemyState currentState;
    public Transform[] patrolPoints;
    public int patrolIndex = 0;
    public float distanceThreshold = 2f;
    public int enemyID;

    // Start is called once before the first execution of Update after the MonoBehaviour is created

    public void SaveGamePrepare()
    {
        //Create enemy save data
        LoadSaveManager.GameStateData.DataEnemy dataEnemy = new LoadSaveManager.GameStateData.DataEnemy();

        //Transform data
        //Position data
        dataEnemy.transform.posX = transform.position.x;
        dataEnemy.transform.posY = transform.position.y;
        dataEnemy.transform.posZ = transform.position.z;
        //Rotation data
        dataEnemy.transform.rotX = transform.rotation.eulerAngles.x;
        dataEnemy.transform.rotY = transform.rotation.eulerAngles.y;
        dataEnemy.transform.rotZ = transform.rotation.eulerAngles.z;
        //Scale data
        dataEnemy.transform.scaleX = transform.localScale.x;
        dataEnemy.transform.scaleY = transform.localScale.y;
        dataEnemy.transform.scaleZ = transform.localScale.z;

        //Health data
        dataEnemy.health = currentHealth;

        //Enemy ID
        dataEnemy.enemyID = enemyID;

        //Add enemy data to the game state
        GameManager.StateManager.gameState.enemies.Add(dataEnemy);

    }

    public void LoadGameComplete()
    {
        //Cycle through all enemies in the saved game data to find the matching ID
        List<LoadSaveManager.GameStateData.DataEnemy> enemies = GameManager.StateManager.gameState.enemies;

        //Reference to this enemy
        LoadSaveManager.GameStateData.DataEnemy dataEnemy = null;

        for(int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].enemyID == enemyID)
            {
                dataEnemy = enemies[i];
                break;
            }
        }

        //If no matching enemy found
        if (dataEnemy == null)
        {
            Destroy(gameObject);
            return;
        }

        //Enemy ID
        enemyID = dataEnemy.enemyID;
        //Health data
        currentHealth = dataEnemy.health;

        //Transform data
        //Position data
        Vector3 pos = new Vector3(dataEnemy.transform.posX, dataEnemy.transform.posY, dataEnemy.transform.posZ);
        transform.position = pos;
        //Rotation data
        Vector3 rot = new Vector3(dataEnemy.transform.rotX, dataEnemy.transform.rotY, dataEnemy.transform.rotZ);
        transform.rotation = Quaternion.Euler(rot);
        //Scale data
        Vector3 scale = new Vector3(dataEnemy.transform.scaleX, dataEnemy.transform.scaleY, dataEnemy.transform.scaleZ);
        transform.localScale = scale;

        enemies.Remove(dataEnemy); //Remove this enemy from the list so it is not loaded again
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        navAgent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();
        currentHealth = maxHealth;

        navAgent.speed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerTransform || GameManager.playerDead)
        {
            if (patrolPoints.Length == 0)
            {
                anim.SetBool("isWalking", false);
                return; // No patrol points, stop the skeleton
            }
            currentState = EnemyState.Patrol;
            target = patrolPoints[patrolIndex];
        }

        if (target == null && patrolPoints.Length == 0)
        {
            anim.SetBool("isWalking", false);
        }



        if (Vector3.Distance(transform.position, playerTransform.position) <= detectionRange && !isDead && !GameManager.playerDead)
        {
            if (minBounds != null && maxBounds != null && IsPlayerWithinBounds())
            {
                anim.SetBool("isWalking", true);
                currentState = EnemyState.Chase;
                target = playerTransform;
            }

            else
            {
                anim.SetBool("isWalking", true);
                currentState = EnemyState.Chase;
                target = playerTransform;
            }
            
        }

        else if (currentState == EnemyState.Patrol && patrolPoints.Length == 0)
            anim.SetBool("isWalking", false);

        else
        {
            currentState = EnemyState.Patrol;
            target = patrolPoints[patrolIndex];
        }

        if (currentState == EnemyState.Patrol && patrolPoints.Length > 0)
        {
            anim.SetBool("isWalking", true);
            if (navAgent.remainingDistance <= distanceThreshold && !navAgent.pathPending)
            {
                patrolIndex = (patrolIndex + 1) % patrolPoints.Length;
                target = patrolPoints[patrolIndex];
            }
        }

        if (currentHealth <= 0)
        {
            if (!anim.GetCurrentAnimatorStateInfo(0).IsName("isDead"))
            {
                isDead = Dead();
                if (isDead)
                {
                    anim.SetTrigger("isDead");
                    GetComponent<CapsuleCollider>().enabled = false;
                    navAgent.isStopped = true; // Stop the NavMeshAgent
                    Destroy(gameObject, 0.5f); // Destroy the skeleton after 0.5 seconds
                    if (itemDrop != null)
                    {
                        GameObject droppedItem = Instantiate(itemDrop, transform.position + new Vector3(0, 1, 0), Quaternion.Euler(0, 0, 270));

                    }
                }
            }
        }
        if (target != null)
            navAgent.SetDestination(target.position);

    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.CompareTag("Kick"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("isKicked")) return;
            anim.SetTrigger("isKicked");
            currentHealth -= 1;
        }

        if (hit.gameObject.CompareTag("Weapon"))
        {
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("isWeaponHit")) return;
            anim.SetTrigger("isWeaponHit");
            currentHealth -= 2;
        }

        if (hit.gameObject.CompareTag("Projectile"))
        {
            Debug.Log("Projectile hit the skeleton");
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("isProjectileHit")) return;
            anim.SetTrigger("isProjectileHit");
            currentHealth -= 1;
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
       
    }

    private bool IsPlayerWithinBounds()
    {
        Vector3 playerPos = playerTransform.position;
        return playerPos.x >= minBounds.x && playerPos.x <= maxBounds.x && playerPos.z >= minBounds.z && playerPos.z <= maxBounds.z;
    }


    bool Dead()
    {
        return true;
    }



}
