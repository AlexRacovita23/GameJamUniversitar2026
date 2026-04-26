using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private List<RecipeData> allRecipes;
    [SerializeField] private ItemData defaultCraftingResult;
    [SerializeField] private int minimumIngredientsForDefault = 2;
    [SerializeField] private GameObject craftingMenu;
    [SerializeField] private CraftingMenuUI menuUI;

    public bool _isCraftingMenuOpen = false;

    private CraftingSpace _craftingSpace;
    private RecipeResolver _recipeResolver;
    private PlayerInputActions _inputActions;

    public CraftingSpace CraftingSpace => _craftingSpace;
    public RecipeResolver RecipeResolver => _recipeResolver;
    public bool IsOpen => menuUI.gameObject.activeSelf;

    public static event System.Action OnCraftingStateChanged;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        _craftingSpace = new CraftingSpace();
        _recipeResolver = new RecipeResolver(allRecipes, defaultCraftingResult, minimumIngredientsForDefault);

        menuUI.Init(Inventory.Instance, _craftingSpace, _recipeResolver);
        menuUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        _inputActions.Player.Disable();
    }

    public void OnCraftingMenuOpened()
    {
        _isCraftingMenuOpen = !craftingMenu.activeSelf;
        craftingMenu.SetActive(_isCraftingMenuOpen);

        OnCraftingStateChanged?.Invoke();
    }
}