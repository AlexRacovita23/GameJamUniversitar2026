using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float runMultiplier;
    [SerializeField] private float interactionRange;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpBuffer;
    [SerializeField] private float stepLength;

    [Header("Camera")]
    [SerializeField] private float mouseSensitivity;
    [SerializeField] private float maxVerticalAngle;
    [SerializeField] private Transform cameraPivot;
    public GameObject pressText;
    public GameObject runtimeUI;

    [Header("Crafting")]
    [SerializeField] private CraftingManager craftingManager;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float yaw;
    private float pitch;
    private bool isRunning;
    private bool isGrounded;
    private bool isMenuOpen;
    private float walkedDistance;
    private bool obeliskAlreadyActivated;

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
        obeliskAlreadyActivated = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
        inputActions.Player.Jump.performed += PerformJump;
        inputActions.Player.Interact.performed += PerformInteract;
        inputActions.Player.Inventory.performed += OpenInventory;
        InventoryUIManager.OnInventoryStateChanged += OnMenuToggled;
        CraftingManager.OnCraftingStateChanged += OnMenuToggled;

        Time.timeScale = 1f; // Ensure the game is not paused when enabled
        ObeliskController.OnObeliskActivated += OnObeliskActivated;
    }

    private void OnDisable()
    {
        inputActions.Player.Jump.performed -= PerformJump;
        inputActions.Player.Interact.performed -= PerformInteract;
        inputActions.Player.Inventory.performed -= OpenInventory;
        InventoryUIManager.OnInventoryStateChanged -= OnMenuToggled;
        CraftingManager.OnCraftingStateChanged -= OnMenuToggled;
        ObeliskController.OnObeliskActivated -= OnObeliskActivated;
        inputActions.Disable();
    }

    private void OnObeliskActivated()
    {
        obeliskAlreadyActivated = true;
        Debug.Log("Obelisk has been activated - further obelisk interactions disabled");
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
                craftingManager.OnCraftingMenuOpened();
            }

            if (hit.collider.CompareTag("Temple"))
            {
                //hit.collider.GetComponent<TempleController>()?.ActivateTemple();
            }

            if (hit.collider.CompareTag("Obelisk") && !obeliskAlreadyActivated)
            {
                var obelisk = hit.collider.GetComponent<ObeliskController>();
                if (obelisk == null)
                {
                    Debug.LogError($"'{hit.collider.name}' has Obelisk tag but no ObeliskController!");
                    return;
                }
                obelisk.TryActivate();
            }
        }
    }

    private void Update()
    {
        ReadValues();
        CheckHit();
        CheckGrounded();

        if (isMenuOpen)
        {
            runtimeUI.SetActive(false);
        }
        else
        {
            runtimeUI.SetActive(true);
        }
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

        if (!isMenuOpen)
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
        if (!isMenuOpen)
            rb.MovePosition(newPosition);

        walkedDistance += move.magnitude * currentMoveSpeed * Time.fixedDeltaTime;
        if (walkedDistance >= stepLength && isGrounded)
        {
            AudioManager.Instance.PlayFootstep(isRunning);
            walkedDistance = 0f;
        }
    }

    private void CheckHit()
    {
        Ray ray = new Ray(cameraPivot.position, cameraPivot.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, interactionRange))
        {
            if ((hit.collider.CompareTag("Collectable") || hit.collider.CompareTag("Crafting") || hit.collider.CompareTag("Obelisk") || hit.collider.CompareTag("Note")) && !isMenuOpen)
            {
                Debug.Log("Hit: " + hit.collider.name);
                if (pressText != null)
                    pressText.SetActive(true);
            }
        }
        else
        {
            if (pressText != null)
                pressText.SetActive(false);
        }
    }

    private void CheckGrounded()
    {
        bool oldGrounded = isGrounded;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, jumpBuffer);
    }

    public void ChangeCoursorState()
    {
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            isMenuOpen = true;
            Time.timeScale = 0f; // Pause the game
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Time.timeScale = 1f; // Resume the game
            isMenuOpen = false;
        }
    }

    private void OnMenuToggled()
    {
        ChangeCoursorState();
    }
}