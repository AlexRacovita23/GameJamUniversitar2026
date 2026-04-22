using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftingMenuUI : MonoBehaviour
{
    [SerializeField] private InventoryPanelUI inventoryPanel;
    [SerializeField] private CraftingTableUI tablePanel;
    [SerializeField] private RectTransform inventoryZone;
    [SerializeField] private RectTransform craftingZone;
    [SerializeField] private Button craftButton;
    [SerializeField] private GameObject resultDisplay;

    private Inventory _inventory;
    private CraftingTable _table;
    private RecipeResolver _resolver;
    private MatLayoutManager _layoutManager;

    private void Awake()
    {
        _layoutManager = GetComponent<MatLayoutManager>();
    }

    public void Init(Inventory inventory, CraftingTable table, RecipeResolver resolver)
    {
        _inventory = inventory;
        _table = table;
        _resolver = resolver;

        inventoryPanel.Init(inventory, OnDraggedToInventory);
        tablePanel.Init(table, resolver, OnDraggedToCrafting);
        craftButton.onClick.AddListener(OnCraftPressed);
    }

    public void OpenInventory()
    {
        gameObject.SetActive(true);
        inventoryPanel.RandomLayout();
        tablePanel.RandomLayout();
    }

    public void CloseInventory()
    {
        gameObject.SetActive(false);
    }

    private void OnDraggedToInventory(DraggableItem draggable)
    {
        ItemData item = draggable.Data;

        if (!_table.Slots.Contains(item))
            return;

        _table.RemoveIngredient(item);
        _inventory.AddItem(item);

        PlaceItemInZone(draggable, inventoryZone, inventoryPanel);

        inventoryPanel.SoftRefresh();
        tablePanel.SoftRefresh();
    }

    private void OnDraggedToCrafting(DraggableItem draggable)
    {
        ItemData item = draggable.Data;

        if (_inventory.GetCount(item) <= 0)
            return;

        if (!_table.TryAddIngredient(item))
            return;

        _inventory.RemoveItem(item);

        PlaceItemInZone(draggable, craftingZone, tablePanel);

        inventoryPanel.SoftRefresh();
        tablePanel.SoftRefresh();
    }

    private void PlaceItemInZone(DraggableItem draggable, RectTransform zone, MonoBehaviour panel)
    {
        draggable.transform.SetParent(zone, true);

        var existingRects = new List<RectTransform>();
        foreach (Transform child in zone)
        {
            var rt = child.GetComponent<RectTransform>();
            if (rt != null && rt != draggable.GetComponent<RectTransform>())
                existingRects.Add(rt);
        }

        RectTransform draggableRect = draggable.GetComponent<RectTransform>();
        Vector2 pos = _layoutManager.GetValidPosition(draggableRect, zone, existingRects);
        draggableRect.localPosition = pos;
    }

    private void OnCraftPressed()
    {
        var recipe = _resolver.TryResolve(_table.Slots);
        if (recipe == null) return;

        _table.Clear();
        _inventory.AddItem(recipe.result);

        inventoryPanel.SoftRefresh();
        tablePanel.SoftRefresh();
    }
}