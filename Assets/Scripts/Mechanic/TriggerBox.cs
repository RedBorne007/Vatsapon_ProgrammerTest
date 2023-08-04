using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

public class TriggerBox : MonoBehaviour
{
    [SerializeField] private UnityEvent onTrigger;

    private bool isTriggered;

    private void OnTriggerEnter()
    {
        if (!isTriggered)
        {
            isTriggered = true;
            onTrigger?.Invoke();
        }
    }
}
