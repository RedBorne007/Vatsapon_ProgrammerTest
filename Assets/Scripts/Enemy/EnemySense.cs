using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySense : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Distance of enemy's FOV")]
    [SerializeField] private float fovDistance = 3f;
    [Tooltip("Distance of enemy's FOV while chasing")]
    [SerializeField] private float fovChaseDistance = 6f;
    [Tooltip("Angle of enemy's FOV")]
    [SerializeField] private float fovAngle = 60f;
    [Tooltip("Angle of enemy's FOV while chasing")]
    [SerializeField] private float fovChaseAngle = 180f;

    [Space]

    [Tooltip("Distance that enemy will sense (ignore FOV)")]
    [SerializeField] private float closeDistance = 2f;
    [Tooltip("Layer that block FOV's vision")]
    [SerializeField] private LayerMask obstacleLayer = ~0;

    [Header("References")]
    [Tooltip("Behaviour of enemy")]
    [SerializeField] private EnemyBehaviour enemy;

    private PlayerController player;

    private void Start()
    {
        player = PlayerController.Instance;
    }

    // Function to determine if player is in sight of enemy's FOV or not.
    public bool IsInSight()
    {
        float fovDistance = enemy.State == EnemyBehaviour.EnemyState.Chase ? fovChaseDistance : this.fovDistance;
        float fovAngle = enemy.State == EnemyBehaviour.EnemyState.Chase ? fovChaseAngle : this.fovAngle;
        float distance = Vector3.Distance(player.transform.position, transform.position);

        Vector3 direction = (player.transform.position - transform.position).normalized;

        // If player's position is within FOV angle...
        if (Vector3.Angle(transform.forward, direction) <= fovAngle / 2f)
        {
            // If player's position is within detection range...
            if (distance <= fovDistance)
            {
                bool isHit = Physics.Raycast(transform.position, direction, distance, obstacleLayer);

                // If there's no obstacle in the way, return true.
                if (!isHit)
                {
                    return true;
                }
            }
        }

        // If distance is too close, return true.
        if (distance <= closeDistance)
        {
            return true;
        }

        return false;
    }
}
