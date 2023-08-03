using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [Tooltip("Image to display item's icon")]
    [SerializeField] private Image iconImage;
    [Tooltip("Object that determine if this item is selected or not")]
    [SerializeField] private GameObject selected;

    private Item item;

    public Item Item => item;

    // Function to set selected/unselected.
    public void SetSelect(bool value) => selected.SetActive(value);

    // Function to set item for this slot.
    public void SetItem(Item item)
    {
        this.item = item;
        iconImage.sprite = item ? item.Sprite : null;
        iconImage.enabled = item;
    }
}
