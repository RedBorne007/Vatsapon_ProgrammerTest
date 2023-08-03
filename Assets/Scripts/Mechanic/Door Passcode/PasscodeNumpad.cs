using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PasscodeNumpad : MonoBehaviour
{
    [Tooltip("Character of this pad")]
    [SerializeField] private string padCharacter;
    [Tooltip("Determine if this pad is for erase")]
    [SerializeField] private bool isErase;

    [Header("References")]
    [Tooltip("Text to display number")]
    [SerializeField] private TMP_Text displayText;
    [Tooltip("Collider of this numpad to press")]
    [SerializeField] private Collider colliders;
    [Tooltip("Passcode that will use this pad")]
    [SerializeField] private PasscodeManager passcodeM;

    [Space]

    [Tooltip("Event when cursor enter pad's collider")]
    [SerializeField] private UnityEvent onEnter;
    [Tooltip("Event when cursor exit pad's collider")]
    [SerializeField] private UnityEvent onExit;

    private bool isPressable = false;

    public UnityEvent OnExit => onExit;
    public bool IsPressable
    {
        get { return isPressable; }
        set
        {
            colliders.enabled = value;
            isPressable = value;
        }
    }

    private void OnMouseEnter()
    {
        if (IsPressable)
        {
            onEnter?.Invoke();
        }
    }

    private void OnMouseExit()
    {
        if (IsPressable)
        {
            onExit?.Invoke();
        }
    }

    private void OnMouseDown()
    {
        if (!IsPressable)
        {
            return;
        }

        if (isErase)
        {
            passcodeM?.Erase();
        }
        else
        {
            passcodeM?.Insert(padCharacter);
        }
    }

    private void OnDrawGizmos()
    {
        // If character length is momre than 1, clamp back to 1 character.
        if (padCharacter.Length > 1)
        {
            padCharacter = padCharacter.Substring(0, 1);
        }

        displayText?.SetText(padCharacter);
    }
}
