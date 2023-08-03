using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : Singleton<InventoryManager>
{
    [Tooltip("Array of all slots")]
    [SerializeField] private InventorySlot[] slots;

    private bool isSelectable = true;
    private int selectIndex;
    
    public bool IsSelectable { get { return isSelectable; } set { isSelectable = value; } }
    public InventorySlot SelectSlot => slots[selectIndex];

    private void Start()
    {
        slots[0].SetSelect(true);
    }

    private void Update()
    {
        if (isSelectable)
        {
            SelectHandler();
        }
    }

    // Function to add item to inventory..
    public bool AddItem(Item item)
    {
        var emptySlot = slots.Where(s => !s.Item);

        // If there's no available slot, return false.
        if (emptySlot.Count() == 0)
        {
            return false;
        }

        emptySlot.First().SetItem(item);
        return true;
    }

    // Function to remove item from inventory.
    public bool RemoveItem(Item item)
    {
        var selectedSlots = slots.Where(s => s.Item && s.Item.Equals(item));

        // If there's no item in any slot, return false.
        if (selectedSlots.Count() == slots.Length)
        {
            return false;
        }

        selectedSlots.First().SetItem(null);
        return true;
    }

    // Function to determine if player has certain item in inventory or not.
    public bool HasItem(Item item) => slots.Where(s => s.Item && s.Item.Equals(item)).Count() > 0;

    // Function to handle selection.
    private void SelectHandler()
    {
        int oldIndex = selectIndex;
        float scrollDelta = Input.mouseScrollDelta.y;

        if (scrollDelta > 0)
        {
            selectIndex--;
        }

        if (scrollDelta < 0)
        {
            selectIndex++;
        }

        selectIndex = Mathf.Clamp(selectIndex, 0, slots.Length - 1);

        // If it's not the same slot, unselect the old slot.
        if (oldIndex != selectIndex)
        {
            slots[oldIndex].SetSelect(false);
        }

        slots[selectIndex].SetSelect(true);
    }
}
