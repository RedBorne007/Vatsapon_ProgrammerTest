using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.UI;

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
    [Tooltip("Image panel of fade screen")]
    [SerializeField] private Image fadeImage;
    [Tooltip("Canvas group that display fade screen")]
    [SerializeField] private CanvasGroup canvasGroup;

    private UnityEvent onUnityAction;
    private Action onAction;
    private bool isFading = false;

    // Function to set color of fading screen.
    public void SetFadeColor(Color color)
    {
        if (fadeImage)
        {
            fadeImage.color = color;
        }
    }

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

    // Function to fade-out.
    public void FadeOut()
    {
        // If it's currently fading, return.
        if (isFading)
        {
            return;
        }

        isFading = true;
        StartCoroutine(FadeOutAsync());
    }

    // Function to set unity action during fade screen.
    public void SetUnityAction(UnityEvent events) => onUnityAction = events;

    // Function to set unity action during fade screen.
    public void SetAction(Action action) => onAction = action;

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

        onUnityAction?.Invoke();
        onAction?.Invoke();
        onAction = () => { };

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

    // Function to fade-out asynchonously.
    private IEnumerator FadeOutAsync()
    {
        canvasGroup.alpha = 1f;

        // Fade-out
        while (canvasGroup.alpha > 0f)
        {
            canvasGroup.alpha -= fadeOutDuration * Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 0f;
        isFading = false;
    }
}
