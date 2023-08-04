using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Statue : MonoBehaviour
{
    [Tooltip("Valid item that need to be place")]
    [SerializeField] private Item validItem;
    [Tooltip("Parent of display object")]
    [SerializeField] private Transform displayParent;
    [Tooltip("Object that display when put item correctly")]
    [SerializeField] private GameObject correctObject;
    [Tooltip("Material that display outline of object")]
    [SerializeField] private Outline outline;
    [Tooltip("Manager of this statue")]
    [SerializeField] private StatueManager statueM;

    private InventoryManager invM;
    private UIManager uiM;

    private ItemObject currentObject;
    private bool isConditioned;

    public bool IsConditioned => isConditioned;

    private void Start()
    {
        invM = InventoryManager.Instance;
        uiM = UIManager.Instance;
    }

    private void OnMouseEnter() => outline.enabled = uiM.IsFocus;
    private void OnMouseExit() => outline.enabled = false;

    private void OnMouseDown()
    {
        // If it's not interactable, return.
        if (!uiM.IsFocus)
        {
            return;
        }

        // If there's item on statue, remove it and replace if selected slot isn't empty.
        if (currentObject)
        {
            // If there's slot to add, add item and destroy object.
            if (invM.AddItem(currentObject.Item))
            {
                AudioManager.Instance.PlaySFX("Cube_Interact");

                correctObject.SetActive(false);
                isConditioned = false;

                Destroy(currentObject.gameObject);
            }
        }
        else if (invM.SelectSlot.Item)
        {
            // If in selected slot in inventory has item, place item.
            GameObject prefab = invM.SelectSlot.Item.Prefab;
            invM.RemoveItem(invM.SelectSlot.Item);
            GameObject itemObject = Instantiate(prefab, displayParent);
            currentObject = itemObject.GetComponent<ItemObject>();

            AudioManager.Instance.PlaySFX("Cube_Interact");

            // If it's the correct item, display correct object.
            if (currentObject.Item.Equals(validItem))
            {
                correctObject.SetActive(true);
                isConditioned = true;

                statueM?.CheckCondition();
            }
        }
    }
}
