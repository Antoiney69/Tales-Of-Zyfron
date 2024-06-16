using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
 
public class MeleeWeapon : MonoBehaviour
{
    PlayerHealth playerHealth;
    //Damage que fait l'arme
    public int damage = 1;
    public GameObject cube;
    public float attackCooldown = 0.5f;
    public bool canAttack ;
    //Détermine quel Layer on peut toucher
    public LayerMask playerLayer;
 
    //Est-ce que l'arme est en train d'être utilisée ?
    public bool isAttacking = false;

    private void Start()
    {
        canAttack = true;
    }


    public void StartAttack()
    {
        isAttacking = true;
    }
 
    public void StopAttack()
    {
        isAttacking = false;
    }
    public void IncreaseDamage(int mult){
        damage*=2*mult;
    }

    //Quand MeleeWeapon entre en collision avec objet
    private void OnTriggerEnter(Collider other)
    {

        if (!isAttacking) return;
        Debug.Log("Enemy weapon collision detected with: " + other.name);
       // if (!isAttacking || !canAttack)
         //  return;
        if (((1 << other.gameObject.layer) & playerLayer) == 0)
           return;

        //Fait des dommages au GameObject qu'on a touché
        if (other.tag == "Player")
        {
            // Attempt to get the PlayerHealth component from the parent GameObject
            playerHealth = other.GetComponent<PlayerHealth>();

            if (playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
                canAttack = false;
                Invoke(nameof(ResetAttack), attackCooldown);
                Debug.Log("Hit");
            }
        }

        //other.GetComponent<PlayerHealth>().TakeDamage(damage);
        //Debug.Log(other.GetComponent<PlayerHealth>().ToString());
   

    }
    private void ResetAttack()
    {
        // Reset attakck
        canAttack = true;
       
    }
    public void Dead(){
        cube.SetActive(false);
    }

}