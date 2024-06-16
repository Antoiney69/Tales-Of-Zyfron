using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class EnemyHp : NetworkBehaviour
{
    [SerializeField] int hp;
    private AnimationHandler animationHandler;
    public bool isDead;
    public int HP { get { return hp; } }
    public NetworkVariable<int> other_HP = new NetworkVariable<int>();
    public int gold;
    public int exp;

    // Start is called before the first frame update
    void Awake()
    {
        isDead = false;
        animationHandler = GetComponent<AnimationHandler>();
    }

    // Update is called once per frame
    void Update()
    {
        other_HP.Value = hp;
        if (hp <= 0) {
            DefeatedServerRpc();
        }

    }
    public void TakeDamage(int damage)
    {
        hp -= damage;
        Debug.Log("enemy took damage");
    }
    
    private void Die()
    {
        isDead = true;
       

    }
    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc(int damage)
    {
        TakeDamage(damage);
    }

    [ServerRpc(RequireOwnership = false)]
    public void DefeatedServerRpc(){
        Die();
    }

    public void IncreaseHP(int mult){
        hp=(int)(hp*1.5f);
    }
    [ServerRpc(RequireOwnership = false)]
    public void IncreaseHPServerRpc(int mult){
        IncreaseHP(mult);
    }
}
