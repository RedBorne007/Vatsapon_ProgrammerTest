using System.Collections;
using System.Collections.Generic;
using System.Globalization;
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
    [Tooltip("Parent of all passcode objects")]
    [SerializeField] private Transform passCodeParents;
    [Tooltip("Interactable of this door")]
    [SerializeField] private BaseInteractable interactable;

    [Space]

    [Tooltip("Event when player able to crack passcode")]
    [SerializeField] private UnityEvent onUnlock;

    private string currentPasscode = "";
    private Coroutine currentErrorCoroutine;
    private bool isUnlock = false;

    // Function to initialize passcode manager.
    public void Initialize()
    {
        // Make sure there's no space in passcode.
        passCode.Replace(" ", "");

        UIManager.Instance.AddClosesFocusListener(delegate
        {
            SetNumpadLock(true);
        });
    }

    // Function to insert passcode.
    public void Insert(string pad)
    {
        // If passcode is already unlocked, return.
        if (isUnlock) 
        {
            return;
        }

        currentPasscode += pad;
        passCodeText?.SetText(currentPasscode);

        // If it reaches maximum passcode's length, check for correction.
        if (currentPasscode.Length == passCode.Length)
        {
            // If the passcode is correct, unlock.
            if (currentPasscode.Equals(passCode))
            {
                isUnlock = true;
                onUnlock?.Invoke();

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

    // Function to set lock/unlock numpad.
    public void SetNumpadLock(bool value)
    {
        for (int i = 0; i < passCodeParents.childCount; i++)
        {
            if (passCodeParents.GetChild(i).TryGetComponent(out PasscodeNumpad numpad))
            {
                numpad.IsPressable = !value;
                numpad.OnExit?.Invoke();
            }
        }
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
