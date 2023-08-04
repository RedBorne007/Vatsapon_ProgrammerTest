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

    private CancellationTokenSource dialogueCancellation;

    // Function to play dialogue.
    public void Play(string text)
    {
        this.text = text;
        
        if (dialogueCancellation != null)
        {
            dialogueCancellation.Cancel();
        }

        dialogueCancellation = new CancellationTokenSource();
        CancellationToken token = dialogueCancellation.Token;

        PlayDialogue(token);
    }

    // Function to handle dialogue sequence.
    private async void PlayDialogue(CancellationToken token)
    {
        currentDisplayDuration = displayDuration;
        dialogueText.text = text;

        await Show();

        while (currentDisplayDuration > 0f)
        {
            if (!Application.isPlaying || token.IsCancellationRequested)
            {
                dialogueCancellation.Dispose();
                return;
            }

            dialoguePanel.alpha = 1f;
            currentDisplayDuration -= Time.deltaTime;
            await Task.Yield();
        }

        await Hide();
        dialogueCancellation = null;
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

            dialoguePanel.alpha += fadeDuration * Time.deltaTime;
            await Task.Yield();
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

            dialoguePanel.alpha -= fadeDuration * Time.deltaTime;
            await Task.Yield();
        }

        dialoguePanel.alpha = 0f;
    }
}
