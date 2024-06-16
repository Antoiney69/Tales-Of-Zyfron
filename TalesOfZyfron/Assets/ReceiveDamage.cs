
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
public class ReceiveDamage : NetworkBehaviour
{
    // Maximum de points de vie
    public int maxHitPoint = 5;
    public Boss_MonsterController boss;
    // Points de vie actuels
    public int hitPoint = 0;

    // Après avoir reçu un dégât :
    // La créature est invulnérable quelques instants
    public bool isInvulnerable;
    private bool isDead = false;
    // Temps d'invulnérabilité
    public float invulnerabilityTime;

    // Temps depuis le dernier dégât
    private float timeSinceLastHit = 0.0f;
    public int gold;
    public int exp;

    private void Start()
    {
        // Au début : Points de vie actuels = Maximum de points de vie
        hitPoint = maxHitPoint;
        isInvulnerable = false;
    }

    private void Update()
    {
        // Si invulnérable
        if (isDead) return;
        if (isInvulnerable)
        {
            // Compte le temps depuis le dernier dégât
            timeSinceLastHit += Time.deltaTime;

            if (timeSinceLastHit > invulnerabilityTime)
            {
                // Le temps est écoulé, il n'est plus invulnérable
                timeSinceLastHit = 0.0f;
                isInvulnerable = false;
            }
        }
    }

    // Permet de recevoir des dommages
    public void GetDamage(int damage)
    {
        if (isInvulnerable)
            return;

        isInvulnerable = true;

        // Applique les dommages aux points de vies actuels
        hitPoint -= damage;

        // S'il reste des points de vie
        /*if (hitPoint > 0)
        {
            // Appelle la méthode TakeDamage sur le GameObject (boss)
            SendMessage("TakeDamage", SendMessageOptions.DontRequireReceiver);
            
        }*/
        // Sinon
        if (hitPoint <= 0)
        {
            // Appelle la méthode Defeated sur le GameObject (boss)
            isDead = true;
            SendMessage("Defeated", SendMessageOptions.DontRequireReceiver);
            boss.Defeated();
            boss.IsDeadClientRpc();
        }
        
        
    }

    [ServerRpc(RequireOwnership = false)]
    public void GetDamageServerRpc(int damage){
        GetDamage(damage);
    }
    public void IncreaseHP(){
        maxHitPoint=(int)(maxHitPoint*1.5f);
        hitPoint = maxHitPoint;
    }
}
