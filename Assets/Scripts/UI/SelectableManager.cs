using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[ExecuteInEditMode]
[RequireComponent(typeof(Selectable))]
public class SelectableManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IPointerUpHandler
{
    [Tooltip("Text that will display on label")]
    [SerializeField] private string labelText = "Button";

    [Header("References")]
    [Tooltip("Label of this selectable")]
    [SerializeField] private TMP_Text label;

    [Space]

    [Tooltip("Event that execute based on button events")]
    [SerializeField] private SelectableManager_Event events;

    private void Update() => label?.SetText(labelText);

    public void OnPointerEnter(PointerEventData eventData) => events.OnEnter?.Invoke();
    public void OnPointerExit(PointerEventData eventData) => events.OnExit?.Invoke();
    public void OnPointerClick(PointerEventData eventData) => events.OnClick?.Invoke();
    public void OnPointerUp(PointerEventData eventData) => events.OnRelease?.Invoke();
}

[System.Serializable]
public struct SelectableManager_Event
{
    [Tooltip("Event when cursor enter the UI area")]
    [SerializeField] private UnityEvent onEnter;
    [Tooltip("Event when cursor exit the UI area")]
    [SerializeField] private UnityEvent onExit;
    [Tooltip("Event when press LMB in the UI area")]
    [SerializeField] private UnityEvent onClick;
    [Tooltip("Event when release LMB form the UI area")]
    [SerializeField] private UnityEvent onRelease;

    public UnityEvent OnEnter => onEnter;
    public UnityEvent OnExit => onExit;
    public UnityEvent OnClick => onClick;
    public UnityEvent OnRelease => onRelease;

}