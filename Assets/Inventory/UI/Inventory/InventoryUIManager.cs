using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private ItemData[] allItems;
    [SerializeField] private GridInventorySlot[] slots;

    private PlayerInputActions _inputActions;

    public static event System.Action<bool> OnInventoryStateChanged;

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
        inventoryPanel.SetActive(false);
    }

    private void OnInventoryToggle(InputAction.CallbackContext context)
    {
        bool isOpen = !inventoryPanel.activeSelf;
        inventoryPanel.SetActive(isOpen);

        OnInventoryStateChanged?.Invoke(isOpen);
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