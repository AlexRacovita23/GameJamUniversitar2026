using System;
using System.Collections.Generic;
using UnityEngine;

public class GridInventoryUI : MonoBehaviour
{
    [Header("Slots - Assign manually placed slots here")]
    [SerializeField] private GridInventorySlot[] slots;

    [Header("Context Menu")]
    [SerializeField] private InventoryContextMenu contextMenu;

    private Inventory _inventory;
    private Dictionary<ItemData, GridInventorySlot> _itemToSlot = new();

    public event Action<ItemData> OnItemConsumed;

    private void Awake()
    {
        if (contextMenu != null)
        {
            contextMenu.Init(HandleConsumeClicked);
        }
    }

    public void Init(Inventory inventory)
    {
        _inventory = inventory;
        _itemToSlot.Clear();

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            slot.Init(this);

            if (slot.AssignedItem != null)
            {
                _itemToSlot[slot.AssignedItem] = slot;
            }
        }

        Refresh();
    }

    public void Refresh()
    {
        if (_inventory == null) return;

        foreach (var kvp in _itemToSlot)
        {
            int count = _inventory.GetCount(kvp.Key);
            kvp.Value.UpdateDisplay(count);
        }
    }

    public void ShowContextMenu(GridInventorySlot slot, Vector2 position)
    {
        if (contextMenu != null)
        {
            contextMenu.Show(slot, position);
        }
    }

    public void HideContextMenu()
    {
        if (contextMenu != null)
        {
            contextMenu.Hide();
        }
    }

    private void HandleConsumeClicked(ItemData item)
    {
        OnItemConsumed?.Invoke(item);
    }
}