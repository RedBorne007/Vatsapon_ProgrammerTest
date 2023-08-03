using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemObject : MonoBehaviour
{
    [Tooltip("Item of this object")]
    [SerializeField] private Item item;
    [Tooltip("Material that display outline of object")]
    [SerializeField] private Outline outline;

    private InventoryManager invM;

    public Item Item => item;

    private void Start()
    {
        invM = InventoryManager.Instance;
    }

    private void OnMouseEnter() => outline.enabled = true;
    private void OnMouseExit() => outline.enabled = false;

    private void OnMouseDown()
    {
        if (item && invM.AddItem(item))
        {
            Destroy(gameObject);
        }
    }
}
