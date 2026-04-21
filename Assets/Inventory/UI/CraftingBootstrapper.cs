using System.Collections.Generic;
using UnityEngine;

public class CraftingBootstrapper : MonoBehaviour
{
    [SerializeField] private List<RecipeData> allRecipes;
    [SerializeField] private List<ItemData> startingItems;
    [SerializeField] private CraftingMenuUI menuUI;

    private void Awake()
    {
        var inventory = new Inventory();
        var table = new CraftingTable();
        var resolver = new RecipeResolver(allRecipes);

        foreach (var item in startingItems)
        {
            inventory.AddItem(item);
            inventory.AddItem(item);
        }

        menuUI.Init(inventory, table, resolver);
    }
}