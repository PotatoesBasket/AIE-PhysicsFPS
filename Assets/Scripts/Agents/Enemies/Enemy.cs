using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    HealthController healthController;
    WaypointController waypointController;
    RagdollController ragdollController;

    Player player;
    PlayerTargeter targetFinder;

    NavMeshAgent NMAgent;
    Animator animator;
    SphereCollider mineTrigger;

    [SerializeField] float walkSpeed = 0;
    [SerializeField] float chaseSpeed = 0;

    [SerializeField] float repeatAttackDelay = 0;
    [SerializeField] float attackDamage = 0;
    [SerializeField] float attackDistance = 0;

    float attackTimer = 0;

    public enum State
    {
        PATROLLING,
        WAITINGFORPATH, // for when the player is in range but there is no valid path
        CHASING,
        ATTACKING,
        DEAD
    }

    State currentState = State.PATROLLING;

    void Awake()
    {
        healthController = GetComponent<HealthController>();
        waypointController = GetComponent<WaypointController>();
        ragdollController = GetComponent<RagdollController>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        targetFinder = GetComponentInChildren<PlayerTargeter>();

        NMAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        mineTrigger = GetComponent<SphereCollider>();
    }

    void Update()
    {
        if (!GameManager.IsPaused)
        {
            switch (currentState)
            {
                case State.PATROLLING:
                    waypointController.TraverseWaypoints();
                    break;

                case State.WAITINGFORPATH:
                    WaitForValidPath();
                    break;

                case State.CHASING:
                    ChasePlayer();
                    break;

                case State.ATTACKING:
                    AttackingPlayer();
                    break;

                case State.DEAD:
                    break;
            }
        }
    }

    public NavMeshPathStatus pathStatus;
    public bool gotPath;
    public bool pathPending;

    void WaitForValidPath()
    {
        Quaternion lookAtPlayer = Quaternion.LookRotation(player.transform.position - transform.position, Vector3.up);
        transform.rotation = Quaternion.Euler(0, lookAtPlayer.eulerAngles.y, 0);

        // update path to player
        NMAgent.SetDestination(player.transform.position);

        pathStatus = NMAgent.pathStatus;
        gotPath = NMAgent.hasPath;
        pathPending = NMAgent.pathPending;

        // check if path is valid
        if (NMAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            // if yes, figure out if enemy should be attacking or chasing
            if (Vector3.Distance(player.transform.position, transform.position) > attackDistance)
            {
                currentState = State.ATTACKING;
                attackTimer = 0;
                NMAgent.isStopped = true;
                animator.SetBool("isIdle", false);
                animator.SetBool("isAttacking", true);
            }
            else
            {
                currentState = State.CHASING;
                attackTimer = 0;
                NMAgent.speed = chaseSpeed;
                NMAgent.isStopped = false;
                animator.SetBool("isIdle", false);
                animator.SetBool("isChasing", true);
            }
        }
        else if (pathPending == false)
        {
            /*Note: For some reason when the player enters an unreachable area, then becomes reachable again,
            the path will not be recalculated until the player crosses into a second reachable pathing node.
            This helps a little bit, I think. I re-baked the map after so it ended up not really mattering anyway.*/

            NavMeshPath path = new NavMeshPath();
            
            if (NMAgent.CalculatePath(player.transform.position, path))
                NMAgent.SetPath(path);
        }
    }

    void ChasePlayer()
    {
        // update path to player
        NMAgent.SetDestination(player.transform.position);

        // check path is still valid
        if (NMAgent.pathStatus != NavMeshPathStatus.PathComplete)
        {
            // if not, switch to waiting state
            currentState = State.WAITINGFORPATH;
            NMAgent.isStopped = true;
            animator.SetBool("isIdle", true);
            return;
        }

        // move towards player
        if (Vector3.Distance(player.transform.position, transform.position) <= attackDistance)
        {
            currentState = State.ATTACKING;
            NMAgent.isStopped = true;
            animator.SetBool("isAttacking", true);
        }
    }

    void AttackingPlayer()
    {
        // check player is still in attacking range
        if (Vector3.Distance(player.transform.position, transform.position) > attackDistance)
        {
            // no, resume chase
            currentState = State.CHASING;
            NMAgent.isStopped = false;
            animator.SetBool("isChasing", true);
            animator.SetBool("isAttacking", false);
            return;
        }

        // in range, check if it's time for another attack
        if (attackTimer <= 0)
        {
            player.HealthController.TakeDamage(attackDamage);
            attackTimer = repeatAttackDelay;
        }

        attackTimer -= Time.deltaTime;
    }

    void OnTargetFound()
    {
        NMAgent.SetDestination(player.transform.position);

        if (NMAgent.pathStatus == NavMeshPathStatus.PathComplete)
        {
            currentState = State.CHASING;
            attackTimer = 0;
            NMAgent.isStopped = false;
            NMAgent.speed = chaseSpeed;
            animator.SetBool("isChasing", true);
        }
        else
        {
            currentState = State.WAITINGFORPATH;
            NMAgent.isStopped = true;
            animator.SetBool("isIdle", true);
        }
    }

    void OnTargetLost()
    {
        currentState = State.PATROLLING;
        waypointController.ResumePatrol();

        NMAgent.speed = walkSpeed;
        animator.SetBool("isChasing", false);
    }

    void OnDeath()
    {
        WinManager.current.RemoveFromList(this);

        currentState = State.DEAD;

        ragdollController.SetRagdollActiveState(true);

        NMAgent.isStopped = true;
        mineTrigger.enabled = false;

        OnDisable(); // unsub from events
    }

    void OnPause()
    {
        if (currentState == State.ATTACKING || currentState == State.WAITINGFORPATH || currentState == State.DEAD)
            NMAgent.isStopped = true;
        else
            NMAgent.isStopped = GameManager.IsPaused;
    }

    void OnEnable()
    {
        healthController.OnDeath += OnDeath;
        targetFinder.OnTargetFound += OnTargetFound;
        targetFinder.OnTargetLost += OnTargetLost;
        GameManager.OnPause += OnPause;
    }

    void OnDisable()
    {
        healthController.OnDeath -= OnDeath;
        targetFinder.OnTargetFound -= OnTargetFound;
        targetFinder.OnTargetLost -= OnTargetLost;
        GameManager.OnPause -= OnPause;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Mine mine))
            mine.Detonate();
    }
}