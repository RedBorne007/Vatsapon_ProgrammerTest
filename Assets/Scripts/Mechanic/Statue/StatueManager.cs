using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class StatueManager : MonoBehaviour
{
    [Tooltip("Interactable of this statues")]
    [SerializeField] private BaseInteractable interactable;
    [Tooltip("Statue that will be use for check condition")]
    [SerializeField] private Statue[] statues;

    [Space]

    [Tooltip("Event to execute when all condition met")]
    [SerializeField] private UnityEvent onConditioned;

    // Function to check condition of all statues.
    public void CheckCondition()
    {
        var correctStatues = statues.Where(s => s.IsConditioned);

        // If all statues met conditioned, leave focus and execute event.
        if (correctStatues.Count() == statues.Length)
        {
            UIManager.Instance.LeaveFocus();
            interactable.IsInteactable = false;

            onConditioned?.Invoke();
        }
    }
}
