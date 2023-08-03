using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExamineInteractable : BaseInteractable
{
    [Header("Settings")]
    [Tooltip("Camera that will be use for inspecting")]
    [SerializeField] private CinemachineVirtualCamera inspectCamera;

    private UIManager uiM;

    protected override void Start()
    {
        base.Start();

        uiM = UIManager.Instance;
    }

    // Function to execute when player interact with this interactable.
    public override void OnInteract()
    {
        base.OnInteract();

        isShow = false;
        colliders.enabled = false;
        
        uiM.SetFocusObject(inspectCamera.gameObject, () =>
        {
            GameManager.Instance.SetCameraLock(false);
            GameManager.Instance.SetCursorLock(true);
            UIManager.Instance.InspectScreen.SetActive(false);
            isShow = true;
            colliders.enabled = true;
        });
    }
}
