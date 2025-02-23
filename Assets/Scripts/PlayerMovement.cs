using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public Transform enemy; // Reference to the enemy

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
    }

    private void FixedUpdate()
    {
        if (enemy == null) return; // Ensure there's an enemy to face

        // Move player based on input
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * speed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Rotate towards the enemy while maintaining Y-axis only
        Vector3 direction = enemy.position - transform.position;
        direction.y = 0; // Keep rotation flat on the horizontal plane

        if (direction != Vector3.zero)
        {
            // Apply a rotation offset to fix the incorrect facing direction
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90, 0);
            rb.MoveRotation(targetRotation);
        }
    }
}

