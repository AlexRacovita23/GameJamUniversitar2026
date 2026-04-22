using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftingTableUI : MonoBehaviour
{
    [SerializeField] private DraggableItem itemSlotPrefab;
    [SerializeField] private RectTransform zone;
    [SerializeField] private TextMeshProUGUI resultPreviewLabel;

    private List<DraggableItem> _slots = new();

    private CraftingTable _table;
    private RecipeResolver _resolver;
    private Action<DraggableItem> _onItemRemovedFromTable;

    private DropZone _dropZone;
    private MatLayoutManager _layoutManager;

    private void Awake()
    {
        _dropZone = GetComponent<DropZone>();
        _layoutManager = GetComponentInParent<MatLayoutManager>();
    }

    public void Init(
        CraftingTable table,
        RecipeResolver resolver,
        Action<DraggableItem> onItemRemovedFromTable)
    {
        _table = table;
        _resolver = resolver;
        _onItemRemovedFromTable = onItemRemovedFromTable;
        _dropZone.OnItemDropped += HandleDrop;
        RandomLayout();
    }

    public void RandomLayout()
    {
        foreach (var slot in _slots)
            Destroy(slot.gameObject);
        _slots.Clear();

        var placedRects = new List<RectTransform>();

        foreach (var item in _table.Slots)
        {
            DraggableItem slot = Instantiate(itemSlotPrefab, zone);
            slot.Init(item, 1, ItemSourceZone.CraftingTable);

            RectTransform slotRect = slot.GetComponent<RectTransform>();
            Vector2 pos = _layoutManager.GetValidPosition(slotRect, zone, placedRects);
            slotRect.localPosition = pos;

            placedRects.Add(slotRect);
            _slots.Add(slot);
        }

        RebuildOtherItemsLists();
        UpdatePreviewLabel();
    }

    public void SoftRefresh()
    {
        var neededItems = new List<ItemData>(_table.Slots);
        var slotsToKeep = new List<DraggableItem>();
        var slotsToRemove = new List<DraggableItem>();

        foreach (var slot in _slots)
        {
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
            Destroy(slot.gameObject);
        }

        foreach (var item in neededItems)
        {
            DraggableItem slot = Instantiate(itemSlotPrefab, zone);
            slot.Init(item, 1, ItemSourceZone.CraftingTable);

            RectTransform slotRect = slot.GetComponent<RectTransform>();

            var existingRects = new List<RectTransform>();
            foreach (var s in _slots)
                existingRects.Add(s.GetComponent<RectTransform>());

            Vector2 pos = _layoutManager.GetValidPosition(slotRect, zone, existingRects);
            slotRect.localPosition = pos;
            _slots.Add(slot);
        }

        RebuildOtherItemsLists();
        UpdatePreviewLabel();
    }

    private void RebuildOtherItemsLists()
    {
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

    private void UpdatePreviewLabel()
    {
        if (resultPreviewLabel == null) return;
        var recipe = _resolver.TryResolve(_table.Slots);
        resultPreviewLabel.text = recipe != null ? $"Will craft: {recipe.result.itemName}" : "";
    }

    private void HandleDrop(DraggableItem draggable)
    {
        _onItemRemovedFromTable?.Invoke(draggable);
    }

    private void OnDestroy()
    {
        if (_dropZone != null)
            _dropZone.OnItemDropped -= HandleDrop;
    }
}