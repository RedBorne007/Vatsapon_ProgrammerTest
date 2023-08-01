using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    [Tooltip("Third-person camera that look around player")]
    [SerializeField] private CinemachineFreeLook cameraView;

    private bool isPause = false;

    public bool IsPause => isPause;

    private void Start()
    {
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

    // Function to quit the game.
    public void Quit() => Application.Quit();

    // Function to set pause value.
    public void SetPause(bool value)
    {
        SetCursorLock(!value);
        SetCameraLock(value);

        isPause = value;
        Time.timeScale = isPause ? 0f : 1f;

        UIManager.Instance.PauseScreen.SetActive(isPause);
    }
}
