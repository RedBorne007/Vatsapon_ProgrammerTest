using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    [Tooltip("Current item that being displayed")]
    [SerializeField] private ItemObject currentObject;

    [Space]

    [Tooltip("Parent of display object")]
    [SerializeField] private Transform displayParent;
    [Tooltip("Material that display outline of object")]
    [SerializeField] private Outline outline;

    private InventoryManager invM;

    private bool isInteractable;

    private void Start()
    {
        invM = InventoryManager.Instance;
    }

    // Function to initialize.
    public void Initialize()
    {
        isInteractable = true;
        UIManager.Instance.AddClosesFocusListener(delegate
        {
            isInteractable = false;
        });
    }

    private void OnMouseEnter() => outline.enabled = isInteractable;
    private void OnMouseExit() => outline.enabled = false;

    private void OnMouseDown()
    {
        // If there's item on statue, remove it and replace if selected slot isn't empty.
        if (currentObject)
        {
            // If there's slot to add, add item and destroy object.
            if (invM.AddItem(currentObject.Item))
            {
                Destroy(currentObject);
            }
        }
        else if (invM.SelectSlot.Item)
        {
            // If in selected slot in inventory has item, place item.
            GameObject prefab = invM.SelectSlot.Item.Prefab;
            invM.RemoveItem(invM.SelectSlot.Item);
            GameObject itemObject = Instantiate(prefab, displayParent);
            currentObject = itemObject.GetComponent<ItemObject>();
        }

    }
}
