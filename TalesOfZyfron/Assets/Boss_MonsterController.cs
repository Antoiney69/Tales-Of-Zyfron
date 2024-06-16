using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Unity.Netcode;

public class Boss_MonsterController : NetworkBehaviour
{
    public GameObject player;
    public GameObject keycard;
    public MeleeWeapon meleeWeapon;
    private ReceiveDamage enemyHp;
    private GameManager gameManager;
    //Agent de Navigation

    private UnityEngine.AI.NavMeshAgent agent;

    //Composants
    Animator animator;

    //Actions possibles

    //Stand ou Idle = attendre
    const string STAND_STATE = "Stand";

    //Re�oit des dommages
    const string TAKE_DAMAGE_STATE = "Damage";

    //Est vaincu
    public const string DEFEATED_STATE = "Defeated";

    //Est en train de marcher
    public const string WALK_STATE = "Walk";

    //Attaque
    public const string ATTACK_STATE = "Attack";

    //M�morise l'action actuelle
    public string currentAction;

    //Scanner pour trouver des cibles
    public TargetScanner targetScanner;

    //Cible
    public GameObject currentTarget;

    //Temps avant de perdre la cible
    public float delayLostTarget = 10f;

    private float timeLostTarget = 0;

    public LayerMask playerLayer;

    public float range;
    private List<Transform> playersInRange = new List<Transform>();
    [SerializeField] GameObject head;

    private GameObject[] targets;

    private float Distance;
    // ennemy's range
    public float chaseRange = 15;

    public float attackRange = 2.2f;

    public float attackDuration = 2f;

    EnemyHp life;
    GameManager gm;

    InteractionSystem IS;

    private bool isAttacking;

    private bool isTakingDamage = false;
    private bool Played = false;
    private NetworkVariable<bool> isDead = new NetworkVariable<bool>(false);
    public AudioSource audioSource;
    public AudioClip defeated;
    public AudioClip step;




    private void Awake()
    {
        //Au d�part, la cr�ature attend en restant debout
        currentAction = STAND_STATE;

        //R�f�rence vers l'Animator
        animator = GetComponent<Animator>();

        //R�f�rence NavMeshAgent
        //navMeshAgent = GetComponent<NavMeshAgent>();

        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();

        //R�f�rence de Player

        isAttacking = false;



    }

    private void Update()
    {
        if (isAttacking) return; // Do not update while attacking
        if (isDead.Value) return;

        List<GameObject> potentialTargets = GetAlivePlayers();
        bool targetDetected = false;

        foreach (GameObject target in potentialTargets)
        {
            if (targetScanner.Detect(transform, target))
            {
                //Debug.Log("Target Detected: " + target.name);
                targetDetected = true;

                Distance = Vector3.Distance(target.transform.position, transform.position);
                /*agent.destination = target.transform.position;*/
                //Debug.Log(Distance);
                if (isTakingDamage){
                    isTakingDamage = false;
                }
                if (Distance > attackRange && Distance < chaseRange)
                {
                    RotateToTarget(target.transform);
                    Walk(target);
                    return;
                }
                

                
                if (Distance <= attackRange)
                {
                    agent.ResetPath();
                    StartCoroutine(PerformAttack(target));
                    return;
                    
                }
                
                Stand();   
            }
        }

        if (!targetDetected)
        {
            if (agent.isOnNavMesh)
            {
                agent.ResetPath();
            }
            Stand();
        }
    }

    private List<GameObject> GetAlivePlayers()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Player");
        List<GameObject> sortedTargets = new List<GameObject>();

        // Populate sortedTargets with all players
        foreach (GameObject player in targets)
        {
            sortedTargets.Add(player);
        }

        // Sort the list by distance from closest to farthest
        sortedTargets.Sort((a, b) =>
        {
            float distanceToA = Vector3.Distance(transform.position, a.transform.position);
            float distanceToB = Vector3.Distance(transform.position, b.transform.position);
            return distanceToA.CompareTo(distanceToB);
        });

        return sortedTargets;
    }

        [ClientRpc]
        public void DelayedDestroyClientRpc(){
            StartCoroutine(DelayedDestroy());
        }

    [ServerRpc(RequireOwnership = false)]
    private void FindingTargetServerRpc()
    {
        FindingTarget();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackServerRpc()
    {
        Attack();
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeDamageServerRpc()
    {
        TakeDamage();
    }

    [ServerRpc(RequireOwnership = false)]
    public void DefeatedServerRpc()
    {
        Defeated();
    }

    [ServerRpc(RequireOwnership = false)]
    private void TakingDamageServerRpc()
    {
        TakingDamage();
    }

    [ServerRpc(RequireOwnership = false)]
    private void AttackingServerRpc()
    {
        Attacking();
    }

    // [ServerRpc(RequireOwnership = false)]
    // private void WalkServerRpc()
    //{
    //  Walk();
    // }

    /*

    [ServerRpc(RequireOwnership = false)]
    private void RotateToTargetServerRpc(Transform targetDirection)
    {
        RotateToTarget(targetDirection);
    }*/

    [ServerRpc(RequireOwnership = false)]
    private void ResetAnimationServerRpc()
    {
        ResetAnimation();
    }
    [ClientRpc]
    public void IsDeadClientRpc(){
        Defeated();
    }



    //La cr�ature attend
    private void Stand()
    {
        //R�initialise les param�tres de l'animator
        ResetAnimation();
        //L'action est maintenant "Stand"
        currentAction = STAND_STATE;
        //Le param�tre "Stand" de l'animator = true
        animator.SetBool("Stand", true);
    }

    public void TakeDamage()
    {
        //R�initialise les param�tres de l'animator
        ResetAnimation();
        //L'action est maintenant "Damage"
        currentAction = TAKE_DAMAGE_STATE;
        //Le param�tre "Damage" de l'animator = true
        animator.SetBool("Damage", true);
        agent.ResetPath();
        isTakingDamage = true;
    }

     public void Defeated()
    {
        //Réinitialise les paramètres de l'animator
        ResetAnimation();
        meleeWeapon.Dead();
        //L'action est maintenant "Defeated"  
        currentAction = DEFEATED_STATE;
        //Le paramètre "Defeated" de l'animator = true
        animator.SetBool(DEFEATED_STATE, true);
        isDead.Value = true;
        agent.ResetPath();
        audioSource.PlayOneShot(defeated);
        DelayedDestroyClientRpc();
        if (keycard!=null){
            keycard.SetActive(true);
        }
        
    }

    // Coroutine to handle delayed destruction
    private IEnumerator DelayedDestroy()
    {
        yield return new WaitForSeconds(10f);
        Destroy(gameObject);
    }
    public void StepSound(){
        audioSource.PlayOneShot(step);
    }


    //Permet de surveiller l'animation lorsque l'on prend un d�g�t
    public void TakingDamage()
    {

        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName(TAKE_DAMAGE_STATE))
        {
            //Compte le temps de l'animation
            //normalizedTime : temps �coul� nomralis� (de 0 � 1).
            //Si normalizedTime = 0 => C'est le d�but.
            //Si normalizedTime = 0.5 => C'est la moiti�.
            //Si normalizedTime = 1 => C'est la fin.


            float normalizedTime = this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime;


            //Fin de l'animation
            if (normalizedTime > 1)
            {
                Stand();
            }

        }

    }

    private void Attacking()
    {
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName(ATTACK_STATE))
        {
            //Compte le temps de l'animation
            //normalizedTime : temps �coul� nomralis� (de 0 � 1).
            //Si normalizedTime = 0 => C'est le d�but.
            //Si normalizedTime = 0.5 => C'est la moiti�.
            //Si normalizedTime = 1 => C'est la fin.




            float normalizedTime = this.animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;


            //Fin de l'animation
            if (normalizedTime > 1)
            {

                meleeWeapon.StopAttack();
                Stand();
                return;
            }

            meleeWeapon.StartAttack();

        }
    }


    //Cherche une cible
    private void FindingTarget()
    {
        // V�rifier si le joueur existe et est actif

        // Si le joueur est d�tect�
        if (targetScanner.Detect(transform, player))
        {
            Debug.Log("PlayerDetected");
            currentTarget = player;
            timeLostTarget = 0;
            return;
        }


        // Si le joueur n'est pas d�tect� ou s'il n'est pas actif, r�initialiser la cible actuelle
        currentTarget = null;

        // Si une cible pr�c�dente �tait d�finie, v�rifier si le temps d'abandon est �coul�
        if (currentTarget != null)
        {
            timeLostTarget += Time.deltaTime;

            if (timeLostTarget > delayLostTarget)
            {
                timeLostTarget = 0;
                currentTarget = null;
            }
        }
    }








    //Walk = Marcher
    private void Walk(GameObject target)
    {
        //R�initialise les param�tres de l'animator
        ResetAnimation();
        //L'action est maintenant "Walk"
        currentAction = WALK_STATE;
        //Le param�tre "Walk" de l'animator = true
        animator.SetBool(WALK_STATE, true);
        if (agent.isOnNavMesh)
        {
            agent.destination = target.transform.position;
        }



    }
    public void EndAttack()
    {
        // Reset attack animation parameter
        animator.SetBool(ATTACK_STATE, false);
    }

    private IEnumerator PerformAttack(GameObject target)
    {
        isAttacking = true;
        agent.isStopped = true;
        Attack();
        meleeWeapon.StartAttack();
        float attackStartTime = 0f;
        while (attackStartTime < attackDuration)
        {
            Distance = Vector3.Distance(target.transform.position, transform.position);
            if (Distance > agent.stoppingDistance + 1)
            {
                EndAttack();
                isAttacking = false;
                agent.isStopped = false;
                yield break;
            }

            RotateToTarget(target.transform);
            attackStartTime += Time.deltaTime;
            yield return null;
        }
        meleeWeapon.StopAttack();
        EndAttack();
        isAttacking = false;
        agent.isStopped = false;

    }



    private void Attack()
    {
        //R�initialise les param�tres de l'animator
        ResetAnimation();
        //L'action est maintenant "Attack"
        currentAction = ATTACK_STATE;
        //Le param�tre "Attack" de l'animator = true
        animator.SetBool(ATTACK_STATE, true);
    }


    private void ResumeChase(GameObject target)
    {
        if (target != null)
        {
            agent.SetDestination(target.transform.position);
            Walk(target);
        }
    }



    //Permet de tout le temps regarder en direction de la cible
    private void RotateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0,direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }
    public void IncreaseDifficulty(int mult){
        meleeWeapon.IncreaseDamage(mult);
        enemyHp.IncreaseHP(mult);
    }




    //R�initialise les param�tres de l'animator
    private void ResetAnimation()
    {
        animator.SetBool(STAND_STATE, false);
        animator.SetBool(TAKE_DAMAGE_STATE, false);
        animator.SetBool(DEFEATED_STATE, false);
        animator.SetBool(WALK_STATE, false);
        animator.SetBool(ATTACK_STATE, false);
    }



}