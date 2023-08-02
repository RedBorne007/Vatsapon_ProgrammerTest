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
    private bool isGameOver;

    private UIManager uiM;

    public bool IsPause => isPause;
    public bool IsGameOver => isGameOver;

    private void Start()
    {
        uiM = UIManager.Instance;
        SetPause(false);
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

        UIManager.Instance.PauseScreen.SetActive(isPause);
    }

    // Function to execute when game over.
    public void GameOver()
    {
        // If it's already game over, return.
        if (isGameOver)
        {
            return;
        }

        isGameOver = true;
        fadePlayer?.StartFade();
    }
}
