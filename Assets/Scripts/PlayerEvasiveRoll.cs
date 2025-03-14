using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEvasiveRoll : MonoBehaviour
{
    [Header("Evasive Roll Settings")]
    public float rollDistance = 4f;      // How far the roll moves
    public float rollDuration = 0.4f;    // Duration of the roll
    public float composureCost = 10f;    // Higher composure drain
    public bool isInvincible = false;    // Blocks all attacks while rolling

    private PlayerInput playerInput;
    private InputAction rollAction;
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    public ComposureBar ComposureBar;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();  // Ensure this is assigned
        animator = GetComponent<Animator>();

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement is NULL in PlayerEvasiveRoll! Make sure this script is on the same GameObject as PlayerMovement.");
        }

        if (animator == null)
        {
            Debug.LogError("Animator is NULL in PlayerEvasiveRoll! Make sure an Animator is attached to the GameObject.");
        }

        rollAction = playerInput.actions["EvasiveRoll"]; // FIXED: Using correct variable name

        if (rollAction == null)
        {
            Debug.LogError("EvasiveRoll action not found in PlayerInput actions!");
        }
    }

    private void OnEnable()
    {
        rollAction.performed += OnEvasiveRoll; // FIXED: Now correctly using rollAction
    }

    private void OnDisable()
    {
        if (rollAction != null)
        {
            rollAction.performed -= OnEvasiveRoll;
        }
    }

    private void OnEvasiveRoll(InputAction.CallbackContext context)
    {
        Debug.Log("Evasive Roll triggered");

        if (ComposureBar == null)
        {
            Debug.LogError("ComposureBar is NULL!");
            return;
        }

        if (playerMovement == null)
        {
            Debug.LogError("playerMovement is NULL!");
            return;
        }

        if (ComposureBar.currentComposure >= composureCost && !playerMovement.isStaggered)
        {
            Debug.Log("Starting Evasive Roll!");
            StartCoroutine(EvasiveRollCoroutine());
        }
    }

    private IEnumerator EvasiveRollCoroutine()
    {
        isInvincible = true;  // Enable full invincibility
        playerMovement.enabled = false; // Disable normal movement
        ComposureBar.ReduceComposure(composureCost); // Drain composure

        animator.SetTrigger("Roll"); // Play the roll animation

        Vector3 rollDirection = new Vector3(playerMovement.moveInput.x, 0, playerMovement.moveInput.y).normalized;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + rollDirection * rollDistance;

        float elapsedTime = 0f;
        while (elapsedTime < rollDuration)
        {
            rb.MovePosition(Vector3.Lerp(startPosition, targetPosition, elapsedTime / rollDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPosition); // Ensure final position is exact
        isInvincible = false; // End invincibility
        playerMovement.enabled = true; // Re-enable movement
    }
}

