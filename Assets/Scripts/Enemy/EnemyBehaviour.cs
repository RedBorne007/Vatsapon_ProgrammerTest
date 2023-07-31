using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol, Check, Chase
    }

    [Tooltip("Determine to use debug mode or not")]
    [SerializeField] private bool isDebug = false;

    [Header("Settings")]
    [Tooltip("Speed for walking/patroling")]
    [SerializeField] private float walkSpeed = 3f;
    [Tooltip("Speed for chasing")]
    [SerializeField] private float chaseSpeed = 5f;
    [Tooltip("Duration to wait before go to next patrol position")]
    [SerializeField] private float patrolWaitDuration = 3f;
    [Tooltip("Duration to look around before go to back to patrol")]
    [SerializeField] private float lookAroundDuration = 3f;

    [Header("References")]
    [Tooltip("State of enemy's behaviour")]
    [SerializeField] private EnemyState enemyState;
    [Tooltip("Rigidbody of enemy")]
    [SerializeField] private Rigidbody rigid;
    [Tooltip("Animator controller of Enemy's model")]
    [SerializeField] private Animator animator;
    [Tooltip("Agent that control enemy's pathfinding")]
    [SerializeField] private NavMeshAgent navAgent;
    [Tooltip("Patrol path for the enemy")]
    [SerializeField] private EnemyPatrol patrol;
    [Tooltip("Sight/Sense handler for the enemy")]
    [SerializeField] private EnemySense sense;

    private const string ANIM_WALK_HASH = "IsWalking";
    private const string ANIM_CHASE_HASH = "IsChasing";
    private const string ANIM_SCREAM = "Enemy_Scream";

    private Vector3 lastSeenPosition; // Lastest position that enemy saw player.
    private float currentChaseDelay;
    private float currentPatrolWaitDuration;
    private float currentLookAroundDuration;

    private PlayerController player;

    public EnemyState State => enemyState;
    public Vector3 LastSeenPosition { get { return lastSeenPosition; } set { lastSeenPosition = value; } }

    private void Start()
    {
        player = PlayerController.Instance;
    }

    private void Update()
    {
        switch (enemyState)
        {
            case EnemyState.Patrol:
            Patrol();
            break;

            case EnemyState.Check:
            Check();
            break;

            case EnemyState.Chase:
            Chase();
            break;
        }

        AnimationHandler();
    }
    
    // Function to handle enemy's animation.
    private void AnimationHandler()
    {
        animator.SetBool(ANIM_WALK_HASH, !navAgent.isStopped && enemyState != EnemyState.Chase);
        animator.SetBool(ANIM_CHASE_HASH, !navAgent.isStopped && enemyState == EnemyState.Chase);
    }

    // Function to handle patrol behaviour.
    private void Patrol()
    {
        navAgent.speed = walkSpeed;
        
        // If there's player in sight, start chasing.
        if (sense.IsInSight())
        {
            lastSeenPosition = player.transform.position;
            ChangeState(EnemyState.Chase);
            return;
        }

        // If there's wait timer, wait until it move again.
        if (currentPatrolWaitDuration > 0f)
        {
            navAgent.isStopped = true;
            currentPatrolWaitDuration -= Time.deltaTime;
            return;
        }

        navAgent.isStopped = false;
        navAgent.SetDestination(patrol.Current);
        float distance = Vector3.Distance(transform.position, patrol.Current);

        // If enemy is close to the patrol position, set next patrol position.
        if (distance <= 0.1f)
        {
            patrol.Next();
            currentPatrolWaitDuration = patrolWaitDuration;
        }
    }

    // Function to handle check behaviour.
    private void Check()
    {

    }

    // Function to handle chase behaviour.
    private void Chase()
    {
        // Delay before start chasing.
        if (currentChaseDelay > 0f)
        {
            currentChaseDelay -= Time.deltaTime;

            if (currentChaseDelay <= 0f)
            {
                lastSeenPosition = player.transform.position;
                navAgent.isStopped = false;
            }

            return;
        }

        navAgent.speed = chaseSpeed;

        // If there's wait timer, wait. If it reaches 0, go back to patrol.
        if (currentLookAroundDuration > 0f)
        {
            // If enemy still find player while looking around, make it still follow player.
            if (sense.IsInSight())
            {
                lastSeenPosition = player.transform.position;
                currentLookAroundDuration = 0f;
            }
            else
            {
                navAgent.isStopped = true;
                currentLookAroundDuration -= Time.deltaTime;
            }

            return;
        }

        // If enemy is looking around but found no one, return back to patrol.
        if (navAgent.isStopped && !sense.IsInSight())
        {
            patrol.SetClosetPatrol(transform.position);
            ChangeState(EnemyState.Patrol);
            return;
        }

        navAgent.isStopped = false;
        navAgent.SetDestination(lastSeenPosition);

        // If player is in sight while chasing, update last seen position.
        if (sense.IsInSight())
        {
            lastSeenPosition = player.transform.position;
        }

        float distance = Vector3.Distance(transform.position, lastSeenPosition);

        // If enemy is close to the patrol position, set next patrol position.
        if (distance <= 0.1f)
        {
            lastSeenPosition = player.transform.position;
            currentLookAroundDuration = lookAroundDuration;
        }
    }

    // Function to execute when enemy's state is changing.
    private void ChangeState(EnemyState newState)
    {
        // If the old and new state is the same, return.
        if (enemyState == newState)
        {
            return;
        }

        switch (newState)
        {
            case EnemyState.Patrol:
            currentPatrolWaitDuration = 0f;
            break;

            case EnemyState.Check:
            break;

            case EnemyState.Chase:

            // Make enemy look toward player's position.
            Vector3 lookDirection = (player.transform.position - transform.position).normalized;
            transform.forward = lookDirection;

            navAgent.isStopped = true;
            currentLookAroundDuration = 0f;

            foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name.Equals(ANIM_SCREAM))
                {
                    currentChaseDelay = clip.length;
                }
            }

            animator.Play(ANIM_SCREAM);

            break;
        }

        enemyState = newState;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(lastSeenPosition, 2f);
    }
}
