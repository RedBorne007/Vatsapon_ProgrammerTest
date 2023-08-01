using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Rendering;

public class EnemyBehavior : MonoBehaviour
{
    public enum EnemyState
    {
        Patrol, Wander, Chase
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
    [Tooltip("Duration to look around before go to back to patrol")]
    [SerializeField] private int wanderTime = 2;
    [Tooltip("Range of wandering and searching")]
    [SerializeField] private float wanderRange = 2;

    [Header("References")]
    [Tooltip("State of enemy's behavior")]
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
    private float currentWanderTime;

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

            case EnemyState.Wander:
            Wander();
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

    // Function to handle patrol behavior (Walk around in certain path).
    private void Patrol()
    {
        navAgent.speed = walkSpeed;
        
        // If there's player in sight, start chasing.
        if (sense.IsInSight())
        {
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

    // Function to handle wander behavior (Look around for player).
    private void Wander()
    {
        navAgent.speed = walkSpeed;

        // If enemy hasn't wander around enough, keep wandering.
        if (currentWanderTime > 0)
        {
            // If there's wait timer, wait until it move again.
            if (currentLookAroundDuration > 0f)
            {
                // If enemy find player, make it start chasing.
                if (sense.IsInSight())
                {
                    ChangeState(EnemyState.Chase);
                    return;
                }

                navAgent.isStopped = true;
                currentLookAroundDuration -= Time.deltaTime;

                // After look around, keep looking other area (If there's wander time left).
                if (currentLookAroundDuration <= 0f)
                {
                    currentWanderTime--;

                    // Assign random position to wander around.
                    Vector2 randomDirection = Random.insideUnitCircle;

                    Physics.Raycast(transform.position + Vector3.up, randomDirection, out RaycastHit hit, wanderRange, sense.ObstacleLayer);

                    // If it hit an obstacle, set that as hit point.
                    if (hit.collider)
                    {
                        lastSeenPosition = hit.point;
                    }
                    else
                    {
                        // Else, use max wander range.
                        lastSeenPosition = new Vector3(randomDirection.x * wanderRange, 0f, randomDirection.y * wanderRange);
                    }
                }

                return;
            }

            // If enemy find player, make it start chasing.
            if (sense.IsInSight())
            {
                ChangeState(EnemyState.Chase);
                return;
            }

            navAgent.isStopped = false;
            navAgent.SetDestination(lastSeenPosition);
            float distance = Vector3.Distance(transform.position, lastSeenPosition);

            // If enemy is close wander position, look around for certain time.
            if (distance <= 0.1f)
            {
                currentLookAroundDuration = lookAroundDuration;
            }
        }
        else
        {
            // Else, go back to patrol.
            patrol.SetClosetPatrol(transform.position);
            ChangeState(EnemyState.Patrol);
        }
    }

    // Function to handle chase behavior (Chasing player when spotted).
    private void Chase()
    {
        // Delay before start chasing.
        if (currentChaseDelay > 0f)
        {
            // Make enemy look toward player's position.
            Vector3 lookDirection = (player.transform.position - transform.position).normalized;
            transform.forward = lookDirection;

            currentChaseDelay -= Time.deltaTime;

            // After delay, make enemy start chasing to latest player's position to catch up.
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

        // If enemy is looking around but found no one, wander around.
        if (navAgent.isStopped && !sense.IsInSight())
        {
            ChangeState(EnemyState.Wander);
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

            case EnemyState.Wander:
            currentWanderTime = wanderTime;
            currentLookAroundDuration = 0f;
            break;

            case EnemyState.Chase:

            navAgent.isStopped = true;
            currentLookAroundDuration = 0f;

            // Set Chase delay based on scream animation's duration.
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
}

#if UNITY_EDITOR
[CustomEditor(typeof(EnemyBehavior))]
public class EnemyBehaviourEditor : Editor
{
    private EnemyBehavior enemy;

    private void OnEnable()
    {
        enemy = target as EnemyBehavior;
    }

    private void OnSceneGUI()
    {
        bool isDebug = serializedObject.FindProperty("isDebug").boolValue;

        if (isDebug)
        {
            Handles.zTest = CompareFunction.LessEqual;

            // Draw player latest's position.
            if (enemy.State != EnemyBehavior.EnemyState.Patrol)
            {
                Color color = enemy.State == EnemyBehavior.EnemyState.Wander ? Color.white : Color.yellow;
                color.a = 0.25f;
                Handles.color = color;
                Handles.DrawSolidDisc(enemy.LastSeenPosition, Vector3.up, 1f);
            }

            // Draw enemy's FOV.
            Object senseObject = serializedObject.FindProperty("sense").objectReferenceValue;

            if (senseObject)
            {
                EnemySense sense = senseObject as EnemySense;

                Color color = Color.white;

                switch (enemy.State)
                {
                    case EnemyBehavior.EnemyState.Wander:
                    color = Color.yellow;
                    break;

                    case EnemyBehavior.EnemyState.Chase:
                    color = Color.red;
                    break;
                }

                color.a = 0.25f;
                Handles.color = color;

                Vector3 forward = enemy.transform.forward;
                forward = Quaternion.AngleAxis(-sense.Angle / 2f, Vector3.up) * forward;

                float angle = enemy.State == EnemyBehavior.EnemyState.Chase ? sense.ChaseAngle : sense.Angle;
                float distance = enemy.State == EnemyBehavior.EnemyState.Chase ? sense.ChaseDistance : sense.Distance;

                Handles.DrawSolidArc(enemy.transform.position, Vector3.up, forward, angle, distance);
            }
        }
    }
}
#endif