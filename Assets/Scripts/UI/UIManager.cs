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
    [Tooltip("Screen that display when inspecting object")]
    [SerializeField] private GameObject inspectScreen;

    private GameObject currentFocusObject;
    private Action onCloseFocus;

    private GameManager gameM;
    private PlayerController player;

    public Transform HUDTransform => hudTransform;
    public GameObject PauseScreen => pauseScreen;
    public GameObject InspectScreen => inspectScreen;

    private void Start()
    {
        gameM = GameManager.Instance;
        player = PlayerController.Instance;
    }

    private void Update()
    {
        // [ESC] - Pause/Unpause the game or get out of focus object.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!currentFocusObject)
            {
                gameM.SetPause(!gameM.IsPause);
            }
            else
            {
                onCloseFocus?.Invoke();
                currentFocusObject.SetActive(false);
                currentFocusObject = null;

                player.SetControllable(true);
            }
        }
    }

    // Function to set focus object (Can be either game object or UI).
    public void SetFocusObject(GameObject screenObject, Action onClose)
    {
        currentFocusObject = screenObject;
        onCloseFocus = onClose;

        currentFocusObject.SetActive(true);
        gameM.SetCursorLock(false);
        gameM.SetCameraLock(true);
        player.SetControllable(false);
    }
}
