using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

[AddComponentMenu("")]
public abstract class BaseInteractable : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Text that will display on popup")]
    [SerializeField] private string interactLabel;
    [Tooltip("Minimum distance to show interactable icon")]
    [SerializeField] private float displayDistance = 5f;
    [Tooltip("Offset of popup position from object's position")]
    [SerializeField] private Vector3 popupOffset;

    [Header("References")]
    [SerializeField] private GameObject interactPopupPrefab;

    [Space]

    [Tooltip("Event to execute when interact this object for the first time")]
    [SerializeField] private UnityEvent onFirstInteract;
    [Tooltip("Event to execute when interact this object")]
    [SerializeField] private UnityEvent onInteract;

    protected bool isInteractable = true; // Determine if it's interactable or not.
    protected bool isShow = true; // Determine to show popup when raycasted or not.
    protected bool isRaycasted; // Determine if player is raycasting this interactable.
    private bool isFirstInteract = false; // Determine if player has interact this once or not.
    private Transform popupParent;
    private GameObject currentPopup;
    private Camera cam;
    private PlayerController player;
    protected Collider colliders;

    public bool IsInteactable { get { return isInteractable; } set { isInteractable = value; } }
    public bool IsShow { get { return isShow; } set { isShow = value; } }

    protected virtual void Start()
    {
        cam = Camera.main;
        player = PlayerController.Instance;
        popupParent = UIManager.Instance.HUDTransform;
        TryGetComponent(out colliders);

        currentPopup = Instantiate(interactPopupPrefab, popupParent);
        currentPopup.transform.SetAsFirstSibling();
        currentPopup.transform.Find("Popup/Panel/Text").GetComponent<TMP_Text>().text = interactLabel;
    }

    protected virtual void Update()
    {
        // If it's not interactable, return.
        if (!isInteractable)
        {
            return;
        }

        bool isInteractKey = PlayerController.Instance.Input.Player.Interact.WasPressedThisFrame();

        if (isInteractKey && isRaycasted)
        {
            OnInteract();
        }

        PopupUpdater();
    }

    // Function to update popup.
    private void PopupUpdater()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        
        Vector3 uiPosition = cam.WorldToScreenPoint(transform.position + popupOffset);
        bool inCameraView = uiPosition.z > 0f;

        currentPopup.transform.position = uiPosition;
        currentPopup.SetActive(distance <= displayDistance && isShow && inCameraView);

        CanvasGroup popupCanvas = currentPopup.transform.Find("Popup").GetComponent<CanvasGroup>();
        popupCanvas.alpha = isRaycasted ? 1f : 0f;
    }

    // Function to set value if it's being raycasted or not.
    public void SetRaycasted(bool value) => isRaycasted = value;

    // Function to execute when player interact with this interactable.
    public virtual void OnInteract()
    {
        if (!isFirstInteract)
        {
            isFirstInteract = true;
            onFirstInteract?.Invoke();
        }

        onInteract?.Invoke();
    }
}
