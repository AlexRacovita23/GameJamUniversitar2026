using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class CraftingMenuUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private InventoryPanelUI inventoryPanel;
    [SerializeField] private CraftingPanelUI craftingPanel;

    [Header("UI Elements")]
    [SerializeField] private Button craftButton;

    private Inventory _inventory;
    private CraftingSpace _craftingSpace;
    private RecipeResolver _recipeResolver;

    public void Init(Inventory inventory, CraftingSpace craftingSpace, RecipeResolver recipeResolver)
    {
        _inventory = inventory;
        _craftingSpace = craftingSpace;
        _recipeResolver = recipeResolver;

        inventoryPanel.Init(inventory, OnDraggedToInventory);
        craftingPanel.Init(craftingSpace, OnDraggedToCrafting);
        craftButton.onClick.AddListener(OnCraftPressed);
    }

    public void OpenMenu()
    {
        gameObject.SetActive(true);
        inventoryPanel.RandomLayout();
        craftingPanel.RandomLayout();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick("OpenUI");
    }

    public void CloseMenu()
    {
        gameObject.SetActive(false);
    }

    private void OnDraggedToInventory(DraggableItem draggable)
    {
        if (draggable.SourceZone != ItemSourceZone.CraftingSpace)
            return;

        ItemData item = draggable.Data;

        if (!_craftingSpace.Slots.Contains(item))
            return;

        _craftingSpace.RemoveIngredient(item);
        _inventory.AddItem(item);

        craftingPanel.RemoveSlot(draggable);
        inventoryPanel.SoftRefresh();
    }

    private void OnDraggedToCrafting(DraggableItem draggable)
    {
        if (draggable.SourceZone != ItemSourceZone.Inventory)
            return;

        ItemData item = draggable.Data;

        if (_inventory.GetCount(item) <= 0)
            return;

        if (!_craftingSpace.TryAddIngredient(item))
            return;

        _inventory.RemoveItem(item);

        inventoryPanel.SoftRefresh();
        craftingPanel.SoftRefresh();
    }

    private void OnCraftPressed()
    {
        var recipe = _recipeResolver.TryResolve(_craftingSpace.Slots);
        if (recipe == null) return;

        _craftingSpace.Clear();
        _inventory.AddItem(recipe.result);

        inventoryPanel.SoftRefresh();
        craftingPanel.SoftRefresh();

        if (AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick("Craft");
    }

    private void OnDestroy()
    {
        if (craftButton != null)
            craftButton.onClick.RemoveListener(OnCraftPressed);
    }
}