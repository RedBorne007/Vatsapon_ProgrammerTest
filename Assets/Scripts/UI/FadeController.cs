using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class FadeController : Singleton<FadeController>
{
    [Header("Settings")]
    [Tooltip("Duration to fade-in")]
    [SerializeField] private float fadeInDuration = 1f;
    [Tooltip("Duration to fade-out")]
    [SerializeField] private float fadeOutDuration = 1f;
    [Tooltip("Duration of fade screen")]
    [SerializeField] private float fadePauseDuration = 0.5f;

    [Header("References")]
    [Tooltip("Canvas group that display fade screen")]
    [SerializeField] private CanvasGroup canvasGroup;

    private UnityEvent onAction;
    private bool isFading = false;

    // Function to fade the panel
    public void Fade()
    {
        // If it's currently fading, return.
        if (isFading)
        {
            return;
        }

        isFading = true;
        StartCoroutine(FadeAsync());
    }

    // Function to set action during fade screen.
    public void SetAction(UnityEvent events) => onAction = events;

    // Functon to fade asychonously.
    private IEnumerator FadeAsync()
    {
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 0f;

        // Fade-in.
        while (canvasGroup.alpha < 1f)
        {
            canvasGroup.alpha += fadeInDuration * Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        onAction?.Invoke();

        yield return new WaitForSeconds(fadePauseDuration);

        // Fade-out
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= fadeOutDuration * Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;

        isFading = false;
    }
}
