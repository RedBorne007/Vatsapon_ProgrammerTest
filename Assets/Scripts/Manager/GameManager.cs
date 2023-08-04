using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Tooltip("Third-person camera that look around player")]
    [SerializeField] private CinemachineFreeLook cameraView;
    [Tooltip("Global Volume that handle Post-Processing")]
    [SerializeField] private Volume globalVolume;
    [Tooltip("Player that control fade screen")]
    [SerializeField] private FadePlayer fadePlayer;

    private bool isPause;
    private bool isResult;

    private DialogueManager dialogueM;
    private AudioManager audioM;

    public bool IsPause => isPause;
    public bool IsResult => isResult;

    private void Start()
    {
        dialogueM = DialogueManager.Instance;
        audioM = AudioManager.Instance;

        SetPause(false);

        fadePlayer.StartFadeOut();
        dialogueM.Play("Where... am I?");
        audioM.PlayMusic("Ambience_Normal");
    }

    // Function to unpause/resume the game.
    public void Unpause() => SetPause(false);

    // Function to lock/unlock cursor.
    public void SetCursorLock(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = !value;
    }

    // Function to lock/unlock camera movement.
    public void SetCameraLock(bool value) => cameraView.enabled = !value;

    // Function to restart the game.
    public void Restart() => SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    // Function to quit the game.
    public void Quit() => Application.Quit();

    // Function to set pause value.
    public void SetPause(bool value)
    {
        SetCursorLock(!value);
        SetCameraLock(value);

        isPause = value;
        Time.timeScale = isPause ? 0f : 1f;
        globalVolume.profile.TryGet(out DepthOfField dof);
        
        if (dof)
        {
            dof.active = value;
        }

        UIManager uiM = UIManager.Instance;
        uiM.PauseScreen.SetActive(isPause);

        // If pause screen is closed, enable Main and disable Setting screen.
        if (!value)
        {
            uiM.PauseMainScreen.SetActive(true);
            uiM.PauseSettingScreen.SetActive(false);
        }
    }

    // Function to execute when player escaped.
    public void Escaped()
    {
        // If it's already game over, return.
        if (isResult)
        {
            return;
        }

        audioM.FadeOutMusic();
        isResult = true;
        fadePlayer?.SetFadeColor(Color.white);
        fadePlayer?.SetAction(delegate
        {
            UIManager.Instance.EscapedScreen.SetActive(true);
            SetCameraLock(true);
            SetCursorLock(false);
        });

        fadePlayer?.StartFade();
    }

    // Function to execute when game over.
    public void GameOver()
    {
        // If it's already game over, return.
        if (isResult)
        {
            return;
        }

        audioM.FadeOutMusic();
        isResult = true;
        fadePlayer?.SetAction(delegate
        {
            UIManager.Instance.GameOverScreen.SetActive(true);
            SetCameraLock(true);
            SetCursorLock(false);
        });

        fadePlayer?.StartFade();
    }
}
