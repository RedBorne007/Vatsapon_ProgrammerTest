using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySense : MonoBehaviour
{
    [Tooltip("Determine to use debug mode or not")]
    [SerializeField] private bool isDebug = false;

    [Header("Settings")]
    [Tooltip("Distance of enemy's FOV")]
    [SerializeField] private float fovDistance = 3f;
    [Tooltip("Distance of enemy's FOV while chasing")]
    [SerializeField] private float fovChaseDistance = 6f;
    [Tooltip("Angle of enemy's FOV")]
    [SerializeField] private float fovAngle = 60f;
    [Tooltip("Angle of enemy's FOV while chasing")]
    [SerializeField] private float fovChaseAngle = 180f;
    [Tooltip("Offset for raycast detection")]
    [SerializeField] private Vector3 raycastOffset = Vector3.zero;

    [Space]

    [Tooltip("Distance that enemy will hear when player is walking (ignore FOV)")]
    [SerializeField] private float hearWalkDistance = 3f;
    [Tooltip("Distance that enemy will hear when player is running (ignore FOV)")]
    [SerializeField] private float hearRunDistance = 5f;
    [Tooltip("Distance that enemy will hear when player is crouching (ignore FOV)")]
    [SerializeField] private float hearCrouchDistance = 2f;
    [Tooltip("Layer that block FOV's vision")]
    [SerializeField] private LayerMask obstacleLayer = ~0;

    [Header("References")]
    [Tooltip("Behaviour of enemy")]
    [SerializeField] private EnemyBehavior enemy;

    private PlayerController player;

    public float Distance => fovDistance;
    public float ChaseDistance => fovChaseDistance;
    public float Angle => fovAngle;
    public float ChaseAngle => fovChaseAngle;
    public float HearWalkDistance => hearWalkDistance;
    public float HearRunDistance => hearRunDistance;
    public float HearCrouchDistance => hearCrouchDistance;
    public LayerMask ObstacleLayer => obstacleLayer;

    private void Start()
    {
        player = PlayerController.Instance;
    }

    // Function to determine if player is in sight of enemy's FOV or not.
    public bool IsInSight()
    {
        float fovDistance = enemy.State == EnemyBehavior.EnemyState.Chase ? fovChaseDistance : this.fovDistance;
        float fovAngle = enemy.State == EnemyBehavior.EnemyState.Chase ? fovChaseAngle : this.fovAngle;
        float distance = Vector3.Distance(player.transform.position, transform.position);

        Vector3 direction = (GetPlayerPosition() - (transform.position + raycastOffset)).normalized;

        // If player's position is within FOV angle...
        if (Vector3.Angle(transform.forward, direction) <= fovAngle / 2f)
        {
            // If player's position is within detection range...
            if (distance <= fovDistance && !IsHitObstacle(direction, distance))
            {
                return true;
            }
        }

        // If enemy hear player walking in certain distance, return true.
        if (distance <= hearWalkDistance && player.IsWalking && !IsHitObstacle(direction, distance))
        {
            return true;
        }

        // If enemy hear player running in certain distance, return true.
        if (distance <= hearRunDistance && player.IsRunning && !IsHitObstacle(direction, distance))
        {
            return true;
        }

        // If enemy hear player crouching in certain distance, return true.
        if (distance <= hearCrouchDistance && player.IsCrouching && player.IsWalking && !IsHitObstacle(direction, distance))
        {
            return true;
        }

        return false;
    }

    // Function to determine if raycast hit obstacle in certain direction and distance or not.
    private bool IsHitObstacle(Vector3 direction, float distance)
    {
        return Physics.Raycast(transform.position + raycastOffset, direction, distance, obstacleLayer);
    }
    
    // Function to get player's position based on player's state.
    private Vector3 GetPlayerPosition() => player.Detect.position + (Vector3.down * (player.IsCrouching ? 0.5f : 0f));

    private void OnDrawGizmos()
    {
        if (isDebug)
        {
            if (!player && !Application.isPlaying)
            {
                player = FindObjectOfType<PlayerController>();
            }

            Gizmos.DrawLine(transform.position + raycastOffset, GetPlayerPosition());
        }
    }
}
