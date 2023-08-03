using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FadePlayer : MonoBehaviour
{
    [Tooltip("Action that will execute during fade screen for transition")]
    [SerializeField] private UnityEvent onAction;

    private FadeController fader;

    private void Start()
    {
        fader = FadeController.Instance;
    }

    // Function to start fade with action.
    public void StartFade()
    {
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
