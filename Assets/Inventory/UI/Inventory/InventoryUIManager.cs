using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GridInventoryUI gridInventoryUI;
    [SerializeField] private ItemData[] allItems;
    [SerializeField] private CraftingManager craftingManager;

    private PlayerInputActions _inputActions;

    public static event System.Action OnInventoryStateChanged;
    public static event System.Action<ItemData> OnItemConsumed;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Inventory.performed += OnInventoryToggle;
    }

    private void OnDisable()
    {
        _inputActions.Player.Inventory.performed -= OnInventoryToggle;
        _inputActions.Player.Disable();
    }

    private void Start()
    {
        if (gridInventoryUI != null)
        {
            gridInventoryUI.Init(Inventory.Instance);
            gridInventoryUI.OnConsumeRequested += HandleConsumeRequested;
        }

        inventoryPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (gridInventoryUI != null)
        {
            gridInventoryUI.OnConsumeRequested -= HandleConsumeRequested;
        }
    }

    private void HandleConsumeRequested(ItemData item)
    {
        Inventory.Instance.RemoveItem(item);
        OnItemConsumed?.Invoke(item);
        if (AudioManager.Instance != null)
        {
            if (item.ItemName == "GoodPotion" || item.ItemName == "NeutralPotion" || item.ItemName == "BadPotion")
            {
                AudioManager.Instance.PlayUIClick("Drink");
            }
            else AudioManager.Instance.PlayUIClick("Consume");
        }
    }

    private void OnInventoryToggle(InputAction.CallbackContext context)
    {
        if (craftingManager._isCraftingMenuOpen)
            return;

        bool isOpen = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isOpen);

        OnInventoryStateChanged?.Invoke();

        if (isOpen && AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick("OpenUI");
    }
}
