using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftingTableUI : MonoBehaviour
{
    [SerializeField] private DraggableItem itemSlotPrefab;
    [SerializeField] private RectTransform zone;
    [SerializeField] private RectTransform craftButtonExclusionZone;

    private List<DraggableItem> _slots = new();

    private CraftingTable _table;
    private Action<DraggableItem> _onItemDroppedHere;

    private DropZone _dropZone;

    private void Awake()
    {
        _dropZone = GetComponent<DropZone>();
    }

    public void Init(
        CraftingTable table,
        Action<DraggableItem> onItemDroppedHere)
    {
        _table = table;
        _onItemDroppedHere = onItemDroppedHere;
        _dropZone.OnItemDropped += HandleDrop;
        RandomLayout();
    }

    public void RandomLayout()
    {
        foreach (var slot in _slots)
        {
            if (slot != null)
                Destroy(slot.gameObject);
        }
        _slots.Clear();

        var placedRects = new List<RectTransform>();

        foreach (var item in _table.Slots)
        {
            DraggableItem slot = Instantiate(itemSlotPrefab, zone);
            slot.Init(item, 1, ItemSourceZone.CraftingTable);
            slot.ExclusionZone = craftButtonExclusionZone;

            RectTransform slotRect = slot.GetComponent<RectTransform>();
            Vector2 pos = LayoutUtils.GetValidPosition(slotRect, zone, placedRects, craftButtonExclusionZone);
            slotRect.localPosition = pos;

            placedRects.Add(slotRect);
            _slots.Add(slot);
        }

        RebuildOtherItemsLists();
    }

    public void RemoveSlot(DraggableItem slot)
    {
        if (slot == null) return;

        _slots.Remove(slot);
        Destroy(slot.gameObject);
        RebuildOtherItemsLists();
    }

    public void SoftRefresh()
    {
        _slots.RemoveAll(slot => slot == null);

        var neededItems = new List<ItemData>(_table.Slots);
        var slotsToKeep = new List<DraggableItem>();
        var slotsToRemove = new List<DraggableItem>();

        foreach (var slot in _slots)
        {
            if (slot == null)
                continue;

            int idx = neededItems.IndexOf(slot.Data);
            if (idx >= 0)
            {
                slotsToKeep.Add(slot);
                neededItems.RemoveAt(idx);
            }
            else
            {
                slotsToRemove.Add(slot);
            }
        }

        foreach (var slot in slotsToRemove)
        {
            _slots.Remove(slot);
            if (slot != null)
                Destroy(slot.gameObject);
        }

        foreach (var item in neededItems)
        {
            DraggableItem slot = Instantiate(itemSlotPrefab, zone);
            slot.Init(item, 1, ItemSourceZone.CraftingTable);
            slot.ExclusionZone = craftButtonExclusionZone;

            RectTransform slotRect = slot.GetComponent<RectTransform>();

            var existingRects = new List<RectTransform>();
            foreach (var s in _slots)
            {
                if (s != null)
                    existingRects.Add(s.GetComponent<RectTransform>());
            }

            Vector2 pos = LayoutUtils.GetValidPosition(slotRect, zone, existingRects, craftButtonExclusionZone);
            slotRect.localPosition = pos;
            _slots.Add(slot);
        }

        RebuildOtherItemsLists();
    }

    private void RebuildOtherItemsLists()
    {
        _slots.RemoveAll(slot => slot == null);

        var allRects = new List<RectTransform>();
        foreach (var slot in _slots)
            allRects.Add(slot.GetComponent<RectTransform>());

        foreach (var slot in _slots)
        {
            var others = new List<RectTransform>(allRects);
            others.Remove(slot.GetComponent<RectTransform>());
            slot.OtherItems = others;
        }
    }

    private void HandleDrop(DraggableItem draggable)
    {
        _onItemDroppedHere?.Invoke(draggable);
    }

    private void OnDestroy()
    {
        if (_dropZone != null)
            _dropZone.OnItemDropped -= HandleDrop;
    }
}