using System;
using System.Collections.Generic;
using UnityEngine;

public class InventoryPanelUI : MonoBehaviour
{
    [SerializeField] private DraggableItem itemSlotPrefab;
    [SerializeField] private RectTransform zone;

    private Inventory _inventory;
    private Action<ItemData> _onItemDroppedHere;
    private DropZone _dropZone;
    private MatLayoutManager _layoutManager;

    private Dictionary<ItemData, DraggableItem> _slotMap = new();

    private void Awake()
    {
        _dropZone = GetComponent<DropZone>();
        _layoutManager = GetComponentInParent<MatLayoutManager>();
    }

    public void Init(Inventory inventory, Action<ItemData> onItemDroppedHere)
    {
        _inventory = inventory;
        _onItemDroppedHere = onItemDroppedHere;
        _dropZone.OnItemDropped += HandleDrop;
        Refresh();
    }

    public void Refresh()
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

        RefreshOtherItemsLists();
    }

    private void RefreshOtherItemsLists()
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

    private void HandleDrop(ItemData item)
    {
        _onItemDroppedHere?.Invoke(item);
    }

    private void OnDestroy()
    {
        if (_dropZone != null)
            _dropZone.OnItemDropped -= HandleDrop;
    }
}