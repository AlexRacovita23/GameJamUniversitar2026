using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInputActions inputActions;
    private Rigidbody rb;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float runMultiplier = 1.5f;
    [SerializeField] private float interactionRange = 2f;

    [Header("Camera")]
    [SerializeField] private float mouseSensitivity = 10f;
    [SerializeField] private float maxVerticalAngle = 80f;
    [SerializeField] private Transform cameraPivot;

    private Vector2 moveInput;
    private Vector2 lookInput;

    private float yaw;
    private float pitch;
    private bool isRunning;

    private void Awake()
    {
        inputActions = new PlayerInputActions();
        rb = GetComponent<Rigidbody>();

        yaw = transform.eulerAngles.y;
        pitch = cameraPivot.localEulerAngles.x;

        if (pitch > 180f)
            pitch -= 360f;

        Cursor.visible = false;
    }

    private void OnEnable()
    {
        inputActions.Enable();
    }

    private void OnDisable()
    {
        inputActions.Disable();
    }

    private void Update()
    {
        ReadValues();
        CheckHit();
    }

    private void FixedUpdate()
    {
        rb.MoveRotation(Quaternion.Euler(0f, yaw, 0f));

        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y);
        move = Quaternion.Euler(0f, yaw, 0f) * move;

        float currentMoveSpeed = isRunning ? moveSpeed * runMultiplier : moveSpeed;
        Vector3 newPosition = rb.position + move * currentMoveSpeed * Time.fixedDeltaTime;
        rb.MovePosition(newPosition);
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

        cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
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
        }
    }
}