using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BaseInteractable : MonoBehaviour
{
    [SerializeField] private string interactLabel;

    [Header("References")]
    [SerializeField] private GameObject interactCanvas;
    [SerializeField] private TMP_Text interactText;

    [SerializeField] private UnityEvent onInteract;

    protected bool isRaycasted; // Determine if player is raycasting this interactable.

    protected virtual void Start()
    {
        interactText.text = interactLabel;
    }

    protected virtual void Update()
    {
        interactCanvas.SetActive(isRaycasted);
    }
    
    // Function to execute when player interact with this interactable.
    public virtual void OnInteract()
    {
        onInteract?.Invoke();
    }
}
