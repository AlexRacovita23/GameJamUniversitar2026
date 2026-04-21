using UnityEngine;

public class CraftingMenuUI : MonoBehaviour
{
    [SerializeField] private InventoryPanelUI inventoryPanel;
    [SerializeField] private CraftingTableUI tablePanel;
    [SerializeField] private Button craftButton;
    [SerializeField] private GameObject resultDisplay;

    private Inventory _inventory;
    private CraftingTable _table;
    private RecipeResolver _resolver;

    public void Init(Inventory inventory, CraftingTable table, RecipeResolver resolver)
    {
        _inventory = inventory;
        _table = table;
        _resolver = resolver;

        inventoryPanel.Init(inventory, OnItemDraggedToTable);
        tablePanel.Init(table, OnItemRemovedFromTable);
        craftButton.onClick.AddListener(OnCraftPressed);
    }

    private void OnItemDraggedToTable(ItemData item)
    {
        if (_table.TryAddIngredient(item))
        {
            _inventory.RemoveItem(item);
            RefreshUI();
        }
    }

    private void OnItemRemovedFromTable(ItemData item)
    {
        _table.RemoveIngredient(item);
        _inventory.AddItem(item);
        RefreshUI();
    }

    private void OnCraftPressed()
    {
        var recipe = _resolver.TryResolve(_table.Slots);
        if (recipe != null)
        {
            _table.Clear();
            _inventory.AddItem(recipe.result);
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        inventoryPanel.Refresh();
        tablePanel.Refresh();
    }
}