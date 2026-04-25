using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private List<RecipeData> allRecipes;

    [Header("References")]
    [SerializeField] private CraftingMenuUI menuUI;

    private CraftingSpace _craftingSpace;
    private RecipeResolver _recipeResolver;

    public CraftingSpace CraftingSpace => _craftingSpace;
    public RecipeResolver RecipeResolver => _recipeResolver;

    private void Start()
    {
        _craftingSpace = new CraftingSpace();
        _recipeResolver = new RecipeResolver(allRecipes);

        menuUI.Init(Inventory.Instance, _craftingSpace, _recipeResolver);
        menuUI.gameObject.SetActive(false);
    }

    public void ToggleMenu()
    {
        if (menuUI.gameObject.activeSelf)
            CloseMenu();
        else
            OpenMenu();
    }

    public void OpenMenu()
    {
        menuUI.OpenMenu();
    }

    public void CloseMenu()
    {
        menuUI.CloseMenu();
    }

    public bool IsOpen => menuUI.gameObject.activeSelf;
}