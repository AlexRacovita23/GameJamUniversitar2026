using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GridInventoryUI : MonoBehaviour
{
    [Header("Slots")]
    [SerializeField] private GridInventorySlot[] slots;

    private Inventory _inventory;
    private Dictionary<ItemData, GridInventorySlot> _itemToSlot = new();

    public event Action<ItemData> OnItemConsumed;

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            HideAllConsumeButtons();
        }
    }

    public void Init(Inventory inventory)
    {
        _inventory = inventory;
        _itemToSlot.Clear();

        foreach (var slot in slots)
        {
            if (slot == null) continue;

            slot.Init();
            slot.OnConsumeClicked += HandleConsumeClicked;
            slot.OnSlotRightClicked += HandleSlotRightClicked;

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

    private void HandleSlotRightClicked(GridInventorySlot clickedSlot)
    {
        foreach (var slot in slots)
        {
            if (slot != null && slot != clickedSlot)
            {
                slot.HideConsumeButton();
            }
        }
    }

    public void HideAllConsumeButtons()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.HideConsumeButton();
            }
        }
    }

    private void HandleConsumeClicked(ItemData item)
    {
        OnItemConsumed?.Invoke(item);
    }

    private void OnDestroy()
    {
        foreach (var slot in slots)
        {
            if (slot != null)
            {
                slot.OnConsumeClicked -= HandleConsumeClicked;
                slot.OnSlotRightClicked -= HandleSlotRightClicked;
            }
        }
    }
}