using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CraftingTableUI : MonoBehaviour
{
    [SerializeField] private DraggableItem itemSlotPrefab;
    [SerializeField] private RectTransform zone;

    [SerializeField] private TextMeshProUGUI resultPreviewLabel;

    private Dictionary<ItemData, DraggableItem> _slotMap = new();

    private CraftingTable _table;
    private RecipeResolver _resolver;
    private Action<ItemData> _onItemRemovedFromTable;

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
        Action<ItemData> onItemRemovedFromTable)
    {
        _table = table;
        _resolver = resolver;
        _onItemRemovedFromTable = onItemRemovedFromTable;

        _dropZone.OnItemDropped += HandleDrop;

        Refresh();
    }

    public void Refresh()
    {
        foreach (var slot in _slotMap.Values)
            Destroy(slot.gameObject);
        _slotMap.Clear();

        var placedRects = new List<RectTransform>();

        foreach (var item in _table.Slots)
        {
            DraggableItem slot = Instantiate(itemSlotPrefab, zone);
            slot.Init(item, 1);

            RectTransform slotRect = slot.GetComponent<RectTransform>();
            Vector2 pos = _layoutManager.GetValidPosition(slotRect, zone, placedRects);
            slotRect.localPosition = pos;

            placedRects.Add(slotRect);
            _slotMap[item] = slot;
        }

        RefreshOtherItemsLists();
        UpdatePreviewLabel();
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

    private void UpdatePreviewLabel()
    {
        if (resultPreviewLabel == null) return;

        var recipe = _resolver.TryResolve(_table.Slots);
        resultPreviewLabel.text = recipe != null
            ? $"Will craft: {recipe.result.itemName}"
            : "";
    }

    private void HandleDrop(ItemData item)
    {
        _onItemRemovedFromTable?.Invoke(item);
    }

    private void OnDestroy()
    {
        if (_dropZone != null)
            _dropZone.OnItemDropped -= HandleDrop;
    }
}