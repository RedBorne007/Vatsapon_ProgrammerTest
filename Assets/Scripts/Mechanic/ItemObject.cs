using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ItemObject : MonoBehaviour
{
    [Tooltip("Item of this object")]
    [SerializeField] private Item item;
    [Tooltip("Material that display outline of object")]
    [SerializeField] private Outline outline;

    [Space]

    [Tooltip("Event to execute when player collect this object")]
    [SerializeField] private UnityEvent onCollect;

    private InventoryManager invM;
    private UIManager uiM;

    public Item Item => item;

    private void Start()
    {
        invM = InventoryManager.Instance;
        uiM = UIManager.Instance;
    }

    private void OnMouseEnter() => outline.enabled = uiM.IsFocus;
    private void OnMouseExit() => outline.enabled = false;

    private void OnMouseDown()
    {
        // If player isn't focus on something, can't collect and return.
        if (!uiM.IsFocus)
        {
            return;
        }

        if (item && invM.AddItem(item))
        {
            onCollect?.Invoke();
            Destroy(gameObject);
        }
    }
}
