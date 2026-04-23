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

    private Inventory _inventory;
    private CraftingTable _table;
    private RecipeResolver _resolver;

    private void Awake()
    {
    }

    public void Init(Inventory inventory, CraftingTable table, RecipeResolver resolver)
    {
        _inventory = inventory;
        _table = table;
        _resolver = resolver;

        inventoryPanel.Init(inventory, OnDraggedToInventory);
        tablePanel.Init(table, OnDraggedToCrafting);
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
        if (draggable.SourceZone != ItemSourceZone.CraftingTable)
            return;

        ItemData item = draggable.Data;

        if (!_table.Slots.Contains(item))
            return;

        _table.RemoveIngredient(item);
        _inventory.AddItem(item);

        Destroy(draggable.gameObject);

        inventoryPanel.SoftRefresh();
        tablePanel.SoftRefresh();
    }

    private void OnDraggedToCrafting(DraggableItem draggable)
    {
        if (draggable.SourceZone != ItemSourceZone.Inventory)
            return;

        ItemData item = draggable.Data;

        if (_inventory.GetCount(item) <= 0)
            return;

        if (!_table.TryAddIngredient(item))
            return;

        _inventory.RemoveItem(item);

        inventoryPanel.SoftRefresh();
        tablePanel.SoftRefresh();
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