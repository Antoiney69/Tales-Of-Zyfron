using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Unity.Netcode;
using Unity.VisualScripting;
using System.Globalization;
using static UnityEngine.GraphicsBuffer;
using Unity.Services.Lobbies.Models;

public class RangedEnemyAI : NetworkBehaviour
{
    [Header("Callables")]
    public LayerMask playerLayer;
    public GameObject orientation;
    [SerializeField] GameObject head;
    public GameObject enemySprite;
    Animator animator;
    public GameObject bullet;
    private AnimationHandler animationHandler;
    private EnemyHp enemyHp;
    private GameManager gameManager;
    

    [Header("Variables")]
    //Attack Variables
    [SerializeField] float rotationSpeed = 1f;
    [SerializeField] float attackSpeed = 15f;
    [SerializeField] float bulletSpeed = 8f;
    [SerializeField] float delayBeforeAttack = 0.4f;
    bool canAttack;
    bool Played = false;



    //States
    public float range;
    private List<Transform> playersInRange = new List<Transform>();
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>(false);
    public AudioSource audioSource;
    public AudioClip gunsound;
    public AudioClip deathsound;

    

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        enemyHp = GetComponent<EnemyHp>();
        animator = enemySprite.GetComponent<Animator>();

    }



    private void Start()
    {
        canAttack = true;
        animationHandler.currState = AnimationHandler.EnemyState.Idle;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (gameManager.difficulty!=1){
            IncreaseDifficultyServerRpc(gameManager.difficulty-1);
        }
    }
    

    // Update is called once per frame
    
    void Update()
    {
        if (isDead.Value && !Played){
            animationHandler.currState = AnimationHandler.EnemyState.Dead;
            Played = true;
        }
        if (isDead.Value){
            return;
        }
        if (IsServer){
            if (enemyHp.HP <=0)
            {
                isDead.Value = true;
                audioSource.PlayOneShot(deathsound);
                animationHandler.currState = AnimationHandler.EnemyState.Dead;                
                DelayedDestroyClientRpc();
                return;
            }
        }
        UpdatePlayersInRange();
        
        if (playersInRange.Count == 0)
        {
            animationHandler.currState = AnimationHandler.EnemyState.Idle;
        }
        else
        {  
            Transform closestPlayer = GetClosestPlayer();
            AttackPlayer(closestPlayer);
            animationHandler.currState = AnimationHandler.EnemyState.Attacking;           
          
        }



    }

    [ClientRpc] 
    public void IsDeadClientRpc(){
        if (enemyHp.HP<=0){
            enemyHp.isDead=true;
        }        
    }

        
        [ClientRpc]
        public void DelayedDestroyClientRpc(){
            StartCoroutine(DelayedDestroy());
        }
        private IEnumerator DelayedDestroy()
        {
            yield return new WaitForSeconds(5f);
            Destroy(gameObject);
        }
    private void UpdatePlayersInRange()
    {
        playersInRange.Clear();
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, playerLayer);
        foreach (Collider col in colliders)
        {
            Transform playerTransform = col.transform;

            // Ensure the player is not already in the list
            if (!playersInRange.Contains(playerTransform) && !IsPlayerObstructed(playerTransform) && playerTransform.tag == "Player")
            {
                playersInRange.Add(playerTransform);
            }

        }
    

    }
    private bool IsPlayerObstructed(Transform playerTransform)
    {
        Vector3 direction = playerTransform.position - head.transform.position;
        float rayRange = direction.magnitude;
        RaycastHit hit;

        // Perform a raycast to check for obstacles
        if (Physics.Raycast(head.transform.position, direction, out hit, rayRange, ~playerLayer))
        {
            Debug.DrawRay(head.transform.position, direction, Color.red, 0.5f);
            // If the raycast hits something other than the player, return true (obstructed)
            if (hit.collider.gameObject != playerTransform.gameObject)
            {
                Debug.Log(hit.collider.gameObject);
                return true;
            }
        }
        return false;
    }
    private Transform GetClosestPlayer()
    {
        Transform closestPlayer = null;
        float closestDistanceSqr = Mathf.Infinity;
        foreach (Transform player in playersInRange)
        {
            Vector3 directionToTarget = player.position - transform.position;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                closestPlayer = player;
            } 
        }
        return closestPlayer;

    }
    private void RotateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }

    private void AttackPlayer(Transform player)
    {
        RotateToTarget(player);
        if (canAttack)
        {
            audioSource.PlayOneShot(gunsound);
            Debug.Log("Can attack");
            canAttack = false;
            StartCoroutine(ShootWithDelay(player));
        }



    }
    private void ResetAttack()
    {   
        canAttack = true;
        
    }
    private IEnumerator ShootWithDelay(Transform player)
    {
        animator.SetTrigger("Shoot");
        // Wait for the delay before spawning the bullet
        yield return new WaitForSeconds(delayBeforeAttack);
        // Spawn bullet
        Rigidbody rb = Instantiate(bullet, orientation.transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        rb.velocity = orientation.transform.forward * bulletSpeed;

        Invoke(nameof(ResetAttack), attackSpeed);
    }
    public void IncreaseDifficulty(int mult){
        bullet.GetComponent<Bullet>().IncreaseDamage(mult);
        enemyHp.IncreaseHP(mult);
    }

    [ServerRpc(RequireOwnership=false)]
    public void IncreaseDifficultyServerRpc(int mult){
        IncreaseDifficulty(mult);
    }

    [ClientRpc]
    public void IncreaseDifficultyClientRpc(int mult){
        IncreaseDifficulty(mult);
    }
}
