using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CraftingBootstrapper : MonoBehaviour
{
    [SerializeField] private List<RecipeData> allRecipes;
    [SerializeField] private CraftingMenuUI menuUI;

    [SerializeField]private bool _isInventoryOpen = false;

    private void Awake()
    {

    }
    private void Start()
    {
        var table = new CraftingTable();
        var resolver = new RecipeResolver(allRecipes);

        menuUI.Init(Inventory.Instance, table, resolver);
    }

    public void ToggleInventory()
    {
        if (_isInventoryOpen) {
            menuUI.CloseInventory();
        }
        else
            menuUI.OpenInventory();
        _isInventoryOpen = !_isInventoryOpen;
    }
}