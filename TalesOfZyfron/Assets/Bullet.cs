using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Bullet : NetworkBehaviour
{
    PlayerHealth playerHealth;
    //Damage que fait l'arme
    public int damage = 1;


    public float attackCooldown = 0.25f;
    public bool canAttack;
    //D�termine quel Layer on peut toucher
    public LayerMask playerLayer;
    private float destroyDelay = 0f;
    private void Start()
    {
        canAttack = true;
    }
    private void Update()
    {
        destroyDelay += Time.deltaTime;
    }


    private void OnTriggerEnter(Collider other)
    {


        if (!canAttack)
            return;
        if (((1 << other.gameObject.layer) & playerLayer) != 0)
        {
            if (other.transform.parent != null)
            {
                // Attempt to get the PlayerHealth component from the collided object
                PlayerHealth playerHealth = other.transform.parent.GetComponent<PlayerHealth>();

                // If PlayerHealth component is found, apply damage
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(damage);
                    canAttack = false;
                    Invoke(nameof(ResetAttack), attackCooldown);
                    //Debug.Log("Bullet Hit");
                }
                /*else
                {
                    Debug.Log("PlayerHealth component not found on " + other.gameObject.name);
                }*/
            }



        }
        // Temp delay cause bullet spawns inside enemy
        if (destroyDelay > 0.1f)
            Destroy(gameObject);




    }



    private void ResetAttack()
    {
        // Reset attakck
        canAttack = true;
    }

    public void IncreaseDamage(int mult){
        damage = (int)(damage*1.5f);
    }

    [ServerRpc(RequireOwnership=false)]
    public void IncreaseDamageServerRpc(int mult){
        IncreaseDamage(mult);
    }
}
