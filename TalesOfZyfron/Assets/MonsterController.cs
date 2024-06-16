using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Unity.Netcode;
 
public class MonsterController : NetworkBehaviour
{
    public GameObject player;
 
    public MeleeWeapon meleeWeapon;
 
    //Agent de Navigation

    private UnityEngine.AI.NavMeshAgent agent;
    
    //Composants
    Animator animator;
 
    //Actions possibles
 
    //Stand ou Idle = attendre
    const string STAND_STATE = "Stand";
 
    //Reçoit des dommages
    const string TAKE_DAMAGE_STATE = "Damage";
 
    //Est vaincu
    public const string DEFEATED_STATE = "Defeated";
 
    //Est en train de marcher
    public const string WALK_STATE = "Walk";
 
    //Attaque
    public const string ATTACK_STATE = "Attack";
 
    //Mémorise l'action actuelle
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
    public float chaseRange = 10;

    public float attackRange = 2.2f;

    EnemyHp life;
    GameManager gm;

    InteractionSystem IS;
  


    

 
      

    private void Awake()
    {
        //Au départ, la créature attend en restant debout
        currentAction = STAND_STATE;
 
        //Référence vers l'Animator
        animator = GetComponent<Animator>();
 
        //Référence NavMeshAgent
        //navMeshAgent = GetComponent<NavMeshAgent>();

        agent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
 
        //Référence de Player

     
    }

    private void Update()
    {
        List<GameObject> potentialTargets = GetAlivePlayers();
        bool targetDetected = false;

        foreach (GameObject target in potentialTargets)
        {
            if (targetScanner.Detect(transform, target))
            {
                Debug.Log("Target Detected: " + target.name);
                targetDetected = true;
                agent.destination = target.transform.position;

                Distance = Vector3.Distance(target.transform.position, transform.position);

                if (Distance > attackRange && Distance < chaseRange)
                {
                    RotateToTarget(target.transform);
                    Walk(target);
                }
                else if (Distance <= attackRange)
                {
                    if (!Input.GetKeyDown(KeyCode.G))
                    {
                        
                        Attack();
                        agent.ResetPath();
                    }
                    else if (Input.GetKeyDown(KeyCode.G))
                    {
                        
                        TakeDamage();
                        agent.ResetPath();
                    }
                }
                else 
                {
                    Stand();
                    break; }
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

   

    [ServerRpc(RequireOwnership = false)]
    private void ResetAnimationServerRpc()
    {
        ResetAnimation();
    }

   

 
    //La créature attend
    private void Stand()
    {
        //Réinitialise les paramètres de l'animator
        ResetAnimation();
        //L'action est maintenant "Stand"
        currentAction = STAND_STATE;
        //Le paramètre "Stand" de l'animator = true
        animator.SetBool("Stand", true);
    }
 
    public void TakeDamage()
    {
        //Réinitialise les paramètres de l'animator
        ResetAnimation();
        //L'action est maintenant "Damage"
        currentAction = TAKE_DAMAGE_STATE;
        //Le paramètre "Damage" de l'animator = true
        animator.SetBool("Damage", true);
    }
 
    public void Defeated()
    {
        //Réinitialise les paramètres de l'animator
        ResetAnimation();
        //L'action est maintenant "Defeated"  
        currentAction = DEFEATED_STATE;
        //Le paramètre "Defeated" de l'animator = true
        animator.SetBool(DEFEATED_STATE, true);

        meleeWeapon.StopAttack();
    }
 
 
    //Permet de surveiller l'animation lorsque l'on prend un dégât
    private void TakingDamage()
    {
 
        if (this.animator.GetCurrentAnimatorStateInfo(0).IsName(TAKE_DAMAGE_STATE))
        {
            //Compte le temps de l'animation
            //normalizedTime : temps écoulé nomralisé (de 0 à 1).
            //Si normalizedTime = 0 => C'est le début.
            //Si normalizedTime = 0.5 => C'est la moitié.
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
            //normalizedTime : temps écoulé nomralisé (de 0 à 1).
            //Si normalizedTime = 0 => C'est le début.
            //Si normalizedTime = 0.5 => C'est la moitié.
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
 
 
    /*private bool MovingToTarget(Transform targetTransform)
    {
 
        //Assigne la destination : le joueur
        navMeshAgent.SetDestination(targetTransform.position);
 
        //Si navMeshAgent n'est pas prêt
        if (navMeshAgent.remainingDistance == 1)
            return true;
 
 
        // navMeshAgent.remainingDistance = distance restante pour atteindre la cible (Player)
        // navMeshAgent.stoppingDistance = à quelle distance de la cible l'IA doit s'arrêter 
        // (exemple 2 m pour le corps à sorps) 
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {
 
            if (currentAction != WALK_STATE)
                Walk(targetTransform.gameObject);
 
        }
        else
        {
            //Si arrivé à bonne distance, regarde vers le joueur
            Vector3 directionToTarget = (targetTransform.position - transform.position).normalized;
            RotateToTargetServerRpc(directionToTarget);
            return false;
 
        }
 
        return true;
    }*/


    //Cherche une cible
    private void FindingTarget()
    {
        // Vérifier si le joueur existe et est actif
        
            // Si le joueur est détecté
            if (targetScanner.Detect(transform, player))
            {
                Debug.Log("PlayerDetected");
                currentTarget = player;
                timeLostTarget = 0;
                return;
            }
        
    
        // Si le joueur n'est pas détecté ou s'il n'est pas actif, réinitialiser la cible actuelle
        currentTarget = null;

        // Si une cible précédente était définie, vérifier si le temps d'abandon est écoulé
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
        //Réinitialise les paramètres de l'animator
        ResetAnimation();
        //L'action est maintenant "Walk"
        currentAction = WALK_STATE;
        //Le paramètre "Walk" de l'animator = true
        animator.SetBool(WALK_STATE, true);
        if (agent.isOnNavMesh)
        {
            agent.destination = target.transform.position;
        }

        

    }
 
 
    private void Attack()
    {
        //Réinitialise les paramètres de l'animator
        ResetAnimation();
        //L'action est maintenant "Attack"
        currentAction = ATTACK_STATE;
        //Le paramètre "Attack" de l'animator = true
        animator.SetBool(ATTACK_STATE, true);
    }

    //Permet de tout le temps regarder en direction de la cible
    private void RotateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
    }





    //Réinitialise les paramètres de l'animator
    private void ResetAnimation()
    {
        animator.SetBool(STAND_STATE, false);
        animator.SetBool(TAKE_DAMAGE_STATE, false);
        animator.SetBool(DEFEATED_STATE, false);
        animator.SetBool(WALK_STATE, false);
        animator.SetBool(ATTACK_STATE, false);
    }   
        

 
}