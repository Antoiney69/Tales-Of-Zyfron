using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AnimationHandler : NetworkBehaviour
{
    public EnemyState currState;

    Animator animator;
    const string isShooting = "isShooting";
    const string stand = "Stand";
    public GameObject enemySprite;

    public enum EnemyState
    {
        Idle,
        Attacking,
        Dead,
    }
    private void Awake()
    {
   
        animator = enemySprite.GetComponent<Animator>();
    }

    private void Update()
    {
        switch(currState)
        {
            case EnemyState.Idle:
                animator.SetBool(stand, true);
                animator.SetBool(isShooting, false);
                break;

            case EnemyState.Attacking:
                // Set attacking animation
                animator.SetBool(stand, false);
                break;

            case EnemyState.Dead:
                // Set dead animation
                animator.SetBool("Dead",true);
                animator.SetBool(stand, false);
                animator.SetBool(isShooting, false);
                break;
        }
        
    } 

}
