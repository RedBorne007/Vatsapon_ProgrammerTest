using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    [Tooltip("Determine to use debug mode or not")]
    [SerializeField] private bool isDebug = false;

    [Space]

    [Header("Settings")]
    [Tooltip("Layer that affect by player's interaction raycast")]
    [SerializeField] private LayerMask interactLayer = ~0;
    [Tooltip("Range of player's interaction")]
    [SerializeField] private float interactRange = 5f;

    private BaseInteractable currentInteractable;

    private void Update()
    {
        RaycastHandler();
    }

    // Function to handle raycast for interaction.
    private void RaycastHandler()
    {
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, interactRange, interactLayer);

        // If raycast hit something and has Interactable...
        if (hit.collider && hit.collider.TryGetComponent(out BaseInteractable interactable))
        {
            // If there's current interactable and it's different from new one, replace it.
            if (currentInteractable && !currentInteractable.Equals(interactable))
            {
                currentInteractable.SetRaycasted(false);
                currentInteractable = interactable;
                interactable.SetRaycasted(true);
            }

            // If there's no current interactable yet, set the hit one as the current.
            if (!currentInteractable)
            {
                currentInteractable = interactable;
                currentInteractable.SetRaycasted(true);
            }
        }
        else
        {
            // Else, make it un-raycast (if there's current interactable).
            currentInteractable?.SetRaycasted(false);
            currentInteractable = null;
        }
    }

    private void OnDrawGizmos()
    {
        if (isDebug)
        {
            Gizmos.color = currentInteractable ? Color.green : Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + (transform.forward * interactRange));
        }
    }
}
