using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private bool isPause = false;

    private void Start()
    {
        SetPause(false);
    }

    private void Update()
    {
        // [ESC] - Pause/Unpause the game.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SetPause(!isPause);
        }
    }

    // Function to unpause/resume the game.
    public void Unpause() => SetPause(false);

    // Function to quit the game.
    public void Quit() => Application.Quit();

    // Function to set pause value.
    private void SetPause(bool value)
    {
        Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = value;

        isPause = value;
        Time.timeScale = isPause ? 0f : 1f;

        UIManager.Instance.PauseScreen.SetActive(isPause);
    }
}
