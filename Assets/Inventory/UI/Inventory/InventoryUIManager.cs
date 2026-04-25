using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryUIManager : MonoBehaviour
{
    [SerializeField] private GameObject inventoryPanel;

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
    }

    private void OnDisable()
    {
        _inputActions.Player.Inventory.performed -= OnInventoryToggle;
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
}