using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class DialogueManager : Singleton<DialogueManager>
{
    [Header("Settings")]
    [Tooltip("Duration to display dialogue before disappear")]
    [SerializeField] private float displayDuration = 2f;
    [Tooltip("Duration to fade dialogue panel for show/hide")]
    [SerializeField] private float fadeDuration = 1f;

    [Header("References")]
    [Tooltip("Panel that will display dialogue")]
    [SerializeField] private CanvasGroup dialoguePanel;
    [Tooltip("Text that display for dialogue")]
    [SerializeField] private TMP_Text dialogueText;

    private string text;
    private float currentDisplayDuration;

    private CancellationTokenSource cancelToken;

    private void Start()
    {
        cancelToken = new CancellationTokenSource();
    }

    // Function to play dialogue.
    public void Play(string text)
    {
        this.text = text;

        try
        {
            cancelToken?.Cancel();
        }
        catch (ObjectDisposedException) { }

        cancelToken = new CancellationTokenSource();

        PlayDialogue();
    }

    // Function to handle dialogue sequence.
    private async void PlayDialogue()
    {
        currentDisplayDuration = displayDuration;
        dialogueText.text = text;

        await Show();

        while (currentDisplayDuration > 0f)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            try
            {
                dialoguePanel.alpha = 1f;
                currentDisplayDuration -= Time.deltaTime;
                await Task.Yield();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                cancelToken?.Dispose();
            }
        }

        await Hide();
        cancelToken = null;
    }
    
    // Function to show dialogue panel.
    private async Task Show()
    {
        while (dialoguePanel.alpha < 1f)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            try
            {
                dialoguePanel.alpha += fadeDuration * Time.deltaTime;
                await Task.Yield();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                cancelToken?.Dispose();
            }
        }

        dialoguePanel.alpha = 1f;
    }

    // Function to hide dialogue panel.
    private async Task Hide()
    {
        while (dialoguePanel.alpha > 0f)
        {
            if (!Application.isPlaying)
            {
                return;
            }

            try
            {
                dialoguePanel.alpha -= fadeDuration * Time.deltaTime;
                await Task.Yield();
            }
            catch (OperationCanceledException)
            {
                return;
            }
            finally
            {
                cancelToken?.Dispose();
            }
        }

        dialoguePanel.alpha = 0f;
    }
}
