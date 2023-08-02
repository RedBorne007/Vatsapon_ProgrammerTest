using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class PasscodeManager : MonoBehaviour
{
    [Tooltip("The correct passcode order")]
    [SerializeField] private string passCode;
    [Tooltip("Text to display when insert correct passcode")]
    [SerializeField] private string successText = "Success";
    [Tooltip("Text to display when insert incorrect passcode")]
    [SerializeField] private string errorText = "Error";
    [Tooltip("Duration to display error text before disappear")]
    [SerializeField] private float errorDisplayDuration = 1f;
    [Tooltip("Color to display for passcode")]
    [SerializeField] private Color passCodeColor = Color.white;
    [Tooltip("Color to display for success text")]
    [SerializeField] private Color successColor = Color.green;
    [Tooltip("Color to display for error text")]
    [SerializeField] private Color errorColor = Color.red;

    [Header("References")]
    [Tooltip("Text to display current passcode")]
    [SerializeField] private TMP_Text passCodeText;
    [Tooltip("Animator of the door")]
    [SerializeField] private Animator animator;
    [Tooltip("Interactable of this door")]
    [SerializeField] private BaseInteractable interactable;

    [Space]

    [Tooltip("Event when player able to crack passcode")]
    [SerializeField] private UnityEvent onUnlock;

    public const string ANIM_OPEN_HASH = "IsOpen";

    private string currentPasscode = "";
    private Coroutine currentErrorCoroutine;
    private bool isUnlock = false;

    // Function to insert passcode.
    public void Insert(int number)
    {
        // If number isn't in range of 0-9 or already unlocked, return.
        if (number < 0 || number > 9 || isUnlock) 
        {
            return;
        }

        currentPasscode += number;
        passCodeText?.SetText(currentPasscode);

        // If it reaches maximum passcode's length, check for correction.
        if (currentPasscode.Length == passCode.Length)
        {
            // If the passcode is correct, unlock.
            if (currentPasscode.Equals(passCode))
            {
                isUnlock = true;
                onUnlock?.Invoke();
                animator.SetBool(ANIM_OPEN_HASH, true);

                passCodeText.text = successText;
                passCodeText.color = successColor;

                interactable.IsShow = false;
                interactable.IsInteactable = false;

                UIManager.Instance.LeaveFocus();
            }
            else
            {
                currentPasscode = "";
                currentErrorCoroutine = StartCoroutine(ErrorDisplay());
            }

            return;
        }

        // If it's currently showing error text, cancel.
        if (currentErrorCoroutine != null)
        {
            StopCoroutine(currentErrorCoroutine);
            passCodeText.color = passCodeColor;
        }
    }

    // Function to erase passcode.
    public void Erase()
    {
        if (string.IsNullOrEmpty(currentPasscode))
        {
            currentPasscode = "";
        }
        else
        {
            currentPasscode = currentPasscode.Remove(currentPasscode.Length - 1, 1);
        }

        passCodeText.text = currentPasscode;
    }

    // Function to display error text with certain time.
    private IEnumerator ErrorDisplay()
    {
        passCodeText.text = errorText;
        passCodeText.color = errorColor;

        yield return new WaitForSeconds(errorDisplayDuration);

        passCodeText.text = "";
        passCodeText.color = passCodeColor;

        currentErrorCoroutine = null;
    }
}
