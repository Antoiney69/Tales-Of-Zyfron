using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class SwordDamage : NetworkBehaviour
{
    public int damage;
    public GameObject player;
    public AudioClip slash;
    public AudioSource audioSource;

    private void OnTriggerEnter(Collider other) 
    {
        Debug.Log("Collision detected with: " + other.name);
        
        if(Input.GetKey(KeyCode.Mouse0) && other.tag == "Ennemy")
        {
            ReceiveDamage receiveDamage = other.GetComponent<ReceiveDamage>();
            EnemyHp ehp = other.GetComponent<EnemyHp>();
            audioSource.PlayOneShot(slash);
            if (receiveDamage != null)
            {
                if (receiveDamage.hitPoint-damage ==0 ){
                    player.GetComponent<Ressources>().UpdateRessources(receiveDamage.gold,receiveDamage.exp);
                }
                Debug.Log("Enemy found, applying damage");
                receiveDamage.GetDamageServerRpc(damage);
            }
            
            if (ehp != null)
            {
                if (ehp.other_HP.Value-damage ==0 ){
                    player.GetComponent<Ressources>().UpdateRessources(ehp.gold,ehp.exp);
                }
                Debug.Log("Enemy found, applying damage");
                ehp.TakeDamageServerRpc(damage);
            }
        }
    }

    public void IncreaseDamage(){
        damage+=1;
    }
}


