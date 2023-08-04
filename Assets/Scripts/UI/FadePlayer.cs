using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadePlayer : MonoBehaviour
{
    [Tooltip("Action that will execute during fade screen for transition")]
    [SerializeField] private UnityEvent onUnityAction;

    private Action onAction;
    private FadeController fader;

    private void Start()
    {
        fader = FadeController.Instance;
    }

    // Function to set color of fading screen.
    public void SetFadeColor(Color color) => fader?.SetFadeColor(color);

    // Function to set action during fade screen.
    public void SetAction(Action action) => onAction = action;

    // Function to start fade with action.
    public void StartFade()
    {
        fader?.SetUnityAction(onUnityAction);
        fader?.SetAction(onAction);
        fader?.Fade();
    }

    // Function to start fade-out.
    public void StartFadeOut()
    {
        if (!fader)
        {
            fader = FadeController.Instance;
        }

        fader?.FadeOut();
    }
}
