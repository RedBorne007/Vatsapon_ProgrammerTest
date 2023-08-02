using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PasscodeNumpad : MonoBehaviour
{
    [Range(0, 9)]
    [Tooltip("Number of passcode pad")]
    [SerializeField] private int number;
    [Tooltip("Determine if this pad is for erase")]
    [SerializeField] private bool isErase;

    [Header("References")]
    [Tooltip("Text to display number")]
    [SerializeField] private TMP_Text displayText;
    [Tooltip("Passcode that will use this pad")]
    [SerializeField] private PasscodeManager passcodeM;

    [Space]

    [Tooltip("Event when cursor enter pad's collider")]
    [SerializeField] private UnityEvent onEnter;
    [Tooltip("Event when cursor exit pad's collider")]
    [SerializeField] private UnityEvent onExit;

    private void OnMouseEnter() => onEnter?.Invoke();
    private void OnMouseExit() => onExit?.Invoke();
    private void OnMouseDown()
    {
        if (isErase)
        {
            passcodeM?.Erase();
        }
        else
        {
            passcodeM?.Insert(number);
        }
    }

    private void OnDrawGizmos()
    {
        if (!isErase)
        {
            displayText?.SetText(number.ToString());
        }
    }
}
