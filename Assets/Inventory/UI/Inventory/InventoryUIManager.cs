using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GridInventoryUI gridInventoryUI;
    [SerializeField] private ItemData[] allItems;

    private PlayerInputActions _inputActions;

    public static event System.Action<bool> OnInventoryStateChanged;
    public static event System.Action<ItemData> OnItemConsumed;

    private void Awake()
    {
        _inputActions = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _inputActions.Player.Enable();
        _inputActions.Player.Inventory.performed += OnInventoryToggle;
        _inputActions.Player.DebugAddAll.performed += OnAddAllItemsDebug;
        _inputActions.Player.DebugRemoveAll.performed += OnRemoveAllItemsDebug;
    }

    private void OnDisable()
    {
        _inputActions.Player.Inventory.performed -= OnInventoryToggle;
        _inputActions.Player.DebugAddAll.performed -= OnAddAllItemsDebug;
        _inputActions.Player.DebugRemoveAll.performed -= OnRemoveAllItemsDebug;
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
            AudioManager.Instance.PlayUIClick("Consume");
    }

    private void OnInventoryToggle(InputAction.CallbackContext context)
    {
        bool isOpen = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isOpen);

        OnInventoryStateChanged?.Invoke(isOpen);

        if (isOpen && AudioManager.Instance != null)
            AudioManager.Instance.PlayUIClick("OpenUI");
    }

    public void OnAddAllItemsDebug(InputAction.CallbackContext context)
    {
        foreach (var item in allItems)
        {
            for (int i = 0; i < 10; i++)
            {
                Inventory.Instance.AddItem(item);
            }
        }
    }

    public void OnRemoveAllItemsDebug(InputAction.CallbackContext context)
    {
        foreach (var item in allItems)
        {
            while (Inventory.Instance.GetCount(item) > 0)
            {
                Inventory.Instance.RemoveItem(item);
            }
        }
    }
}