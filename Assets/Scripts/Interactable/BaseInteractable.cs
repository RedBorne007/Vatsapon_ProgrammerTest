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
    [Tooltip("Offset of popup position from object's position")]
    [SerializeField] private Vector3 popupOffset;

    [Header("References")]
    [SerializeField] private GameObject interactPopupPrefab;

    [Space]

    [SerializeField] private UnityEvent onFirstInteract;
    [SerializeField] private UnityEvent onInteract;

    protected bool isShow = true; // Determine to show popup when raycasted or not.
    protected bool isRaycasted; // Determine if player is raycasting this interactable.
    private bool isFirstInteract = false; // Determine if player has interact this once or not.
    private Transform popupParent;
    private GameObject currentPopup;
    private Camera cam;

    public bool IsShow { get { return isShow; } set { isShow = value; } }

    protected virtual void Start()
    {
        cam = Camera.main;
        popupParent = UIManager.Instance.HUDTransform;

        currentPopup = Instantiate(interactPopupPrefab, popupParent);
        currentPopup.transform.SetAsFirstSibling();
        currentPopup.transform.Find("Text").GetComponent<TMP_Text>().text = interactLabel;
    }

    protected virtual void Update()
    {
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
        Vector2 uiPosition = cam.WorldToScreenPoint(transform.position + popupOffset);
        currentPopup.transform.position = uiPosition;
        currentPopup.SetActive(isRaycasted && isShow);
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
