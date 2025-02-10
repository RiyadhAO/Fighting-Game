using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;

    private Vector2 moveInput;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        // Get the Move action from the PlayerInput component
        moveAction = playerInput.actions["Move"];
    }

    private void OnEnable()
    {
        // Subscribe to the Move action
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
    }

    private void OnDisable()
    {
        // Unsubscribe from the Move action
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
        Debug.Log(gameObject.name + " Move Input: " + moveInput);
    }

    private void FixedUpdate()
    {
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);
    }
}