using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    [Tooltip("Transform that contains all HUDs")]
    [SerializeField] private Transform hudTransform;

    [Tooltip("Screen that display when pause the game")]
    [SerializeField] private GameObject pauseScreen;
    [Tooltip("Screen that display when game over")]
    [SerializeField] private GameObject gameOverScreen;
    [Tooltip("Screen that display when inspecting object")]
    [SerializeField] private GameObject inspectScreen;

    private GameObject currentFocusObject;
    private event Action onCloseFocus;

    private GameManager gameM;
    private PlayerController player;

    public Transform HUDTransform => hudTransform;
    public GameObject PauseScreen => pauseScreen;
    public GameObject GameOverScreen => gameOverScreen;
    public GameObject InspectScreen => inspectScreen;

    private void Start()
    {
        gameM = GameManager.Instance;
        player = PlayerController.Instance;
    }

    private void Update()
    {
        // If it's game over, return.
        if (gameM.IsGameOver)
        {
            return;
        }

        // [ESC] - Pause/Unpause the game or get out of focus object.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            // If there's no focus object, use it as pause/unpause.
            if (!currentFocusObject)
            {
                gameM.SetPause(!gameM.IsPause);
            }
            else
            {
                // Else, unfocus the current object.
                LeaveFocus();
            }
        }
    }

    // Function to set focus object (Can be either game object or UI).
    public void SetFocusObject(GameObject screenObject, Action onClose)
    {
        currentFocusObject = screenObject;
        AddClosesFocusListener(onClose);

        currentFocusObject.SetActive(true);
        gameM.SetCursorLock(false);
        gameM.SetCameraLock(true);
        player.SetControllable(false);
    }

    // Function to add listener when focus object closed.
    public void AddClosesFocusListener(Action action) => onCloseFocus += action;

    // Function to exit from focus object.
    public void LeaveFocus()
    {
        onCloseFocus?.Invoke();
        onCloseFocus = () => { };
        currentFocusObject?.SetActive(false);
        currentFocusObject = null;

        player.SetControllable(true);
    }
}
