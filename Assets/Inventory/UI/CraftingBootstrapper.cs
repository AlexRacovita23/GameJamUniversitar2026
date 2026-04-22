using System.Collections.Generic;
using UnityEngine;

public class CraftingBootstrapper : MonoBehaviour
{
    [SerializeField] private List<RecipeData> allRecipes;
    [SerializeField] private List<ItemData> startingItems;
    [SerializeField] private CraftingMenuUI menuUI;

    private void Awake()
    {

    }
    private void Start()
    {
        var table = new CraftingTable();
        var resolver = new RecipeResolver(allRecipes);

        foreach (var item in startingItems)
        {
            Inventory.Instance.AddItem(item);
            Inventory.Instance.AddItem(item);
        }

        menuUI.Init(Inventory.Instance, table, resolver);
    }
}