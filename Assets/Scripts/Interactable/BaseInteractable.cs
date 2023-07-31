using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class BaseInteractable : MonoBehaviour
{
    [SerializeField] private string interactLabel;

    [Header("References")]
    [SerializeField] private GameObject interactPopupPrefab;

    [SerializeField] private UnityEvent onInteract;

    protected bool isRaycasted; // Determine if player is raycasting this interactable.
    private GameObject currentPopup;

    protected virtual void Start()
    {
        currentPopup = Instantiate(interactPopupPrefab);
        currentPopup.transform.Find("Text").GetComponent<TMP_Text>().text = interactLabel;
    }

    protected virtual void Update()
    {
        CanvasUpdater();
    }

    // Function to update canvas.
    private void CanvasUpdater()
    {
        currentPopup.SetActive(isRaycasted);
    }

    // Function to set value if it's being raycasted or not.
    public void SetRaycasted(bool value) => isRaycasted = value;

    // Function to execute when player interact with this interactable.
    public virtual void OnInteract()
    {
        onInteract?.Invoke();
    }
}
