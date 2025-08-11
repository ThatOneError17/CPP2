using System.Collections;
using UnityEngine;

[RequireComponent(typeof(BoxCollider), typeof(Rigidbody))]

public class WeaponBase : MonoBehaviour
{

    [SerializeField] private Vector3 localPositionValue = Vector3.zero; // Local position of the weapon when equipped
    [SerializeField] private Vector3 localRotationValue = new Vector3(180, 0, 0); // Local rotation of the weapon when equipped

    Rigidbody rb;
    BoxCollider bc;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        bc = GetComponent<BoxCollider>();

    }

    public void Equip(Collider playerCollider, Transform weaponAttachPoint)
    {
        rb.isKinematic = true; //Disable physics interactions
        bc.isTrigger = true; //Set the collider to be a trigger
        transform.SetParent(weaponAttachPoint); //Set the weapon's parent to the player's weapon attach point
        transform.localPosition = localPositionValue; //Reset local position
        transform.localRotation = Quaternion.Euler(localRotationValue); //Reset local rotation   
        Physics.IgnoreCollision(playerCollider, bc, true); //Ignore collisions between the player and the weapon's collider

    }

    public void Drop(Collider playerCollider, Transform weaponAttachPoint)
    {
        rb.isKinematic = false;
        bc.isTrigger = false; //Set the collider to be a non-trigger
        transform.SetParent(null); //Remove the weapon's parent
        rb.AddForce(playerCollider.transform.forward * 10.0f, ForceMode.Impulse); //Add an impulse force to the weapon in the direction the player is facing
        StartCoroutine(DropCooldown(playerCollider)); //Start a coroutine to handle the cooldown for dropping the weapon
    }

    IEnumerator DropCooldown(Collider playerCollider)
    {
        yield return new WaitForSeconds(3f); //Wait for 3 seconds before allowing the weapon to be dropped again
        Physics.IgnoreCollision(playerCollider, bc, false); //Stop ignoring collisions between the player and the weapon's collider
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.playerDead || GameManager.endOfLevel || GameManager.gameOver)
        {
            bc.isTrigger = false;
            rb.isKinematic = false;
        }
    }
}
