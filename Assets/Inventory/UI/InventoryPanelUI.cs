using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanelUI : MonoBehaviour
{
    [SerializeField] private DraggableItem itemSlotPrefab;
    [SerializeField] private RectTransform zone;

    private Inventory _inventory;
    private Action<DraggableItem> _onItemDroppedHere;
    private DropZone _dropZone;
    private MatLayoutManager _layoutManager;

    private Dictionary<ItemData, DraggableItem> _slotMap = new();

    private void Awake()
    {
        _dropZone = GetComponent<DropZone>();
        _layoutManager = GetComponentInParent<MatLayoutManager>();
    }

    public void Init(Inventory inventory, Action<DraggableItem> onItemDroppedHere)
    {
        _inventory = inventory;
        _onItemDroppedHere = onItemDroppedHere;
        _dropZone.OnItemDropped += HandleDrop;
    }

    public void RandomLayout()
    {
        foreach (var slot in _slotMap.Values)
            Destroy(slot.gameObject);
        _slotMap.Clear();

        var placedRects = new List<RectTransform>();

        foreach (var kvp in _inventory.Items)
        {
            ItemData item = kvp.Key;
            int count = kvp.Value;

            DraggableItem slot = Instantiate(itemSlotPrefab, zone);
            slot.Init(item, count);

            RectTransform slotRect = slot.GetComponent<RectTransform>();
            Vector2 pos = _layoutManager.GetValidPosition(slotRect, zone, placedRects);
            slotRect.localPosition = pos;

            placedRects.Add(slotRect);
            _slotMap[item] = slot;
        }

        RebuildOtherItemsLists();
    }

    public void SoftRefresh()
    {
        var toRemove = new List<ItemData>();
        foreach (var kvp in _slotMap)
        {
            int count = _inventory.GetCount(kvp.Key);
            if (count <= 0)
                toRemove.Add(kvp.Key);
            else
                kvp.Value.SetCount(count);
        }

        foreach (var item in toRemove)
        {
            Destroy(_slotMap[item].gameObject);
            _slotMap.Remove(item);
        }

        foreach (var kvp in _inventory.Items)
        {
            if (!_slotMap.ContainsKey(kvp.Key))
            {
                DraggableItem slot = Instantiate(itemSlotPrefab, zone);
                slot.Init(kvp.Key, kvp.Value);

                RectTransform slotRect = slot.GetComponent<RectTransform>();

                var existingRects = new List<RectTransform>();
                foreach (var s in _slotMap.Values)
                    existingRects.Add(s.GetComponent<RectTransform>());

                Vector2 pos = _layoutManager.GetValidPosition(slotRect, zone, existingRects);
                slotRect.localPosition = pos;

                _slotMap[kvp.Key] = slot;
            }
        }

        RebuildOtherItemsLists();
    }

    private void RebuildOtherItemsLists()
    {
        var allRects = new List<RectTransform>();
        foreach (var slot in _slotMap.Values)
            allRects.Add(slot.GetComponent<RectTransform>());

        foreach (var slot in _slotMap.Values)
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