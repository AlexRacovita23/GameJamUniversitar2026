using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runMultiplier = 1.5f;
    [SerializeField] private float interactionRange = 2f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float jumpBuffer = 0.2f;

    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 10f;
    [SerializeField] private float maxVerticalAngle = 80f;
    [SerializeField] private Transform cameraPivot;

    [Header("Crafting -- assigned in Inspector")]
    [SerializeField] private CraftingBootstrapper craftingBootstrapper;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float yaw;
    private float pitch;
    private bool isRunning;
    private bool isGrounded;
    private bool isMenuOpen;

    //public Action onCollected;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();

        yaw = transform.eulerAngles.y;
        pitch = cameraPivot.localEulerAngles.x;

        if (pitch > 180f)
            pitch -= 360f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        isMenuOpen = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Jump.performed += PerformJump;
        inputActions.Player.Interact.performed += PerformInteract;
        inputActions.Player.Inventory.performed += OpenInventory;
        InventoryUIManager.OnInventoryStateChanged += OnInventoryToggled;
    }

    private void OpenInventory(InputAction.CallbackContext context)
    {
        Debug.Log("Inventory performed");
    }

    private void PerformJump(InputAction.CallbackContext context)
    {
        Debug.Log("Jump performed");
        if (isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    private void PerformInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Interact performed");
        Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            if (hit.collider.CompareTag("Collectable"))
            {
                hit.collider.GetComponent<Collectable>()?.CollectItem();
            }

            if (hit.collider.CompareTag("Crafting"))
            {
                ChangeCoursorState();
                craftingBootstrapper.ToggleInventory();
            }
        }
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= PerformJump;
        inputActions.Player.Interact.performed -= PerformInteract;
        inputActions.Player.Inventory.performed -= OpenInventory;
        InventoryUIManager.OnInventoryStateChanged -= OnInventoryToggled;
        inputActions.Disable();
    }

    private void Update()
    {
        ReadValues();
        CheckHit();
        CheckGrounded();
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    private void ReadValues()
    {
        moveInput = inputActions.Player.Move.ReadValue<Vector2>();
        lookInput = inputActions.Player.Look.ReadValue<Vector2>();
        isRunning = inputActions.Player.Run.IsPressed();

        float mouseX = lookInput.x * mouseSensitivity * Time.deltaTime;
        float mouseY = lookInput.y * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -maxVerticalAngle, maxVerticalAngle);

        if(!isMenuOpen)
        {
            cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        }
    }

    private void MovePlayer()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        move = Quaternion.Euler(0f, yaw, 0f) * move;

        float currentMoveSpeed = isRunning ? moveSpeed * runMultiplier : moveSpeed;
        Vector3 newPosition = rb.position + move * currentMoveSpeed * Time.fixedDeltaTime;
        if(!isMenuOpen)
            rb.MovePosition(newPosition);
    }

    private void CheckHit()
    {
        Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            if (hit.collider.CompareTag("Collectable"))
            {
                Debug.Log("Hit: " + hit.collider.name);
            }
            if (hit.collider.CompareTag("Crafting"))
            {
                Debug.Log("Hit: " + hit.collider.name);
            }
        }
    }

    private void CheckGrounded()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, jumpBuffer);
        //isGrounded = Physics.SphereCast(transform.position, 0.5f, Vector3.down, out RaycastHit hit, 0.1f);
        //Debug.DrawRay(transform.position, Vector3.down * 0.1f, isGrounded ? Color.green : Color.red);
    }

    public void ChangeCoursorState()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isMenuOpen = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            isMenuOpen = false;
        }
    }
    private void OnInventoryToggled(bool isOpen)
    {
        ChangeCoursorState();
    }
}