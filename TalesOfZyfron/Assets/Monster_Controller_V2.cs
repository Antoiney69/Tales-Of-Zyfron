using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using Unity.Netcode;

public class Monster_Controller_V2 : NetworkBehaviour
{


    //Agent de Navigation
    NavMeshAgent navMeshAgent;


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

    private void Awake()
    {
        //Au d�part, la cr�ature attend en restant debout
        currentAction = STAND_STATE;

        //R�f�rence vers l'Animator
        animator = GetComponent<Animator>();

        //R�f�rence NavMeshAgent
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {

        List<GameObject> potentialTargets = GetAlivePlayers();
        bool targetDetected = false;

        foreach (GameObject target in potentialTargets)
        {
            //si la cr�ature est d�faite
            //Elle ne peut rien faire d'autres
            if (currentAction == DEFEATED_STATE)
            {
                navMeshAgent.ResetPath();
                return;
            }


            //Si la cr�ature re�oit des dommages:
            //Elle ne peut rien faire d'autres.
            //Cela servira quand on am�liorera ce script.
            if (currentAction == TAKE_DAMAGE_STATE)
            {
                navMeshAgent.ResetPath();
                TakingDamage();
                return;
            }

                //Est-ce que l'IA se d�place vers le joueur ?
                if (MovingToTarget(target.transform))
                {
                    //En train de marcher
                    return;
                }
                else
                {
                    if (currentAction != ATTACK_STATE && currentAction != TAKE_DAMAGE_STATE)
                    {
                        Attack();
                        return;
                    }
                    if (currentAction == ATTACK_STATE)
                    {
                        Attacking();
                        return;
                    }

                    //Defaut
                    Stand();
                    return;
                }



            
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
    }

    public void Defeated()
    {
        //R�initialise les param�tres de l'animator
        ResetAnimation();
        //L'action est maintenant "Defeated"  
        currentAction = DEFEATED_STATE;
        //Le param�tre "Defeated" de l'animator = true
        animator.SetBool(DEFEATED_STATE, true);
    }


    //Permet de surveiller l'animation lorsque l'on prend un d�g�t
    private void TakingDamage()
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

                /*meleeWeapon.StopAttack();*/
                Stand();
                return;
            }

            /*meleeWeapon.StartAttack();*/

        }
    }


    private bool MovingToTarget(Transform target)
    {

        //Assigne la destination : le joueur
        navMeshAgent.SetDestination(target.position);

        //Si navMeshAgent n'est pas pr�t
        if (navMeshAgent.remainingDistance == 0)
            return true;


        // navMeshAgent.remainingDistance = distance restante pour atteindre la cible (Player)
        // navMeshAgent.stoppingDistance = � quelle distance de la cible l'IA doit s'arr�ter 
        // (exemple 2 m pour le corps � sorps) 
        if (navMeshAgent.remainingDistance > navMeshAgent.stoppingDistance)
        {

            if (currentAction != WALK_STATE)
                Walk();

        }
        else
        {
            //Si arriv� � bonne distance, regarde vers le joueur
            RotateToTarget(target);

            return false;
        }

        return true;
    }


    //Walk = Marcher
    private void Walk()
    {
        //R�initialise les param�tres de l'animator
        ResetAnimation();
        //L'action est maintenant "Walk"
        currentAction = WALK_STATE;
        //Le param�tre "Walk" de l'animator = true
        animator.SetBool(WALK_STATE, true);
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

    //Permet de tout le temps regarder en direction de la cible
    private void RotateToTarget(Transform target)
    {
        Vector3 direction = (target.position - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 3f);
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