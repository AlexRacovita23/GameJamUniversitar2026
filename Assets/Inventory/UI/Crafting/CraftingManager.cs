using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CraftingManager : MonoBehaviour
{
    [SerializeField] private List<RecipeData> allRecipes;
    [SerializeField] private GameObject craftingMenu;
    [SerializeField] private CraftingMenuUI menuUI;

    private CraftingSpace _craftingSpace;
    private RecipeResolver _recipeResolver;
    private PlayerInputActions _inputActions;

    public CraftingSpace CraftingSpace => _craftingSpace;
    public RecipeResolver RecipeResolver => _recipeResolver;
    public bool IsOpen => menuUI.gameObject.activeSelf;

    public static event System.Action<bool> OnCraftingStateChanged;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void Start()
    {
        _craftingSpace = new CraftingSpace();
        _recipeResolver = new RecipeResolver(allRecipes);

        menuUI.Init(Inventory.Instance, _craftingSpace, _recipeResolver);
        menuUI.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.DebugCrafting.performed += OnDebugCraftingPressed;
    }

    private void OnDisable()
    {
        _inputActions.Player.DebugCrafting.performed -= OnDebugCraftingPressed;
        _inputActions.Player.Disable();
    }

    private void OnDebugCraftingPressed(InputAction.CallbackContext context)
    {
        bool isOpen = !craftingMenu.activeSelf;
        craftingMenu.SetActive(isOpen);

        OnCraftingStateChanged?.Invoke(isOpen);
    }
    public void OnCraftingMenuOpened()
    {
        bool isOpen = !craftingMenu.activeSelf;
        craftingMenu.SetActive(isOpen);

        OnCraftingStateChanged?.Invoke(isOpen);
    }
}