using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
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

    [Tooltip("Event to execute when cursor enter pad's collider")]
    [SerializeField] private UnityEvent onEnter;
    [Tooltip("Event to execute when cursor exit pad's collider")]
    [SerializeField] private UnityEvent onExit;
    [Tooltip("Event to execute when press on pad's collider")]
    [SerializeField] private UnityEvent onPress;

    private UIManager uiM;

    public Collider Collider => colliders;

    private void Start()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        uiM = UIManager.Instance;
        colliders.enabled = false;
    }

    private void Update()
    {
        // If character length is momre than 1, clamp back to 1 character.
        if (padCharacter.Length > 1)
        {
            padCharacter = padCharacter.Substring(0, 1);
        }

        displayText?.SetText(padCharacter);
    }

    private void OnMouseEnter()
    {
        if (uiM.IsFocus)
        {
            onEnter?.Invoke();
        }
    }

    private void OnMouseExit() => onExit?.Invoke();

    private void OnMouseDown()
    {
        if (!uiM.IsFocus)
        {
            return;
        }

        onPress?.Invoke();

        if (isErase)
        {
            passcodeM?.Erase();
        }
        else
        {
            passcodeM?.Insert(padCharacter);
        }
    }
}
