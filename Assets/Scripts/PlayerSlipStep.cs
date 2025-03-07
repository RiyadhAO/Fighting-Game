using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSlipStep : MonoBehaviour
{
    [Header("Slip Step Settings")]
    public float slipStepDistance = 3f;       // How far the dash moves
    public float slipStepDuration = 0.15f;    // Duration of the dash
    public float ComposureCost = 10f;           // Stamina drained per slip step
    public bool isInvincible = false;         // Tracks invincibility frames

    private PlayerInput playerInput;
    private InputAction slipStepAction;
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    public ComposureBar ComposureBar;            // Reference to stamina system

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();  // Assign playerMovement

        if (playerInput == null)
        {
            Debug.LogError("PlayerInput is NULL in PlayerSlipStep!");
            return;
        }

        slipStepAction = playerInput.actions["SlipStep"];

        if (slipStepAction == null)
        {
            Debug.LogError("SlipStep action not found in PlayerInput actions!");
        }

        if (playerMovement == null)
        {
            Debug.LogError("PlayerMovement is NULL in PlayerSlipStep! Make sure this script is on the same GameObject as PlayerMovement.");
        }
    }




    private void OnEnable()
    {
        slipStepAction.performed += OnSlipStep;
    }

    private void OnDisable()
    {
        if (slipStepAction != null)
        {
            slipStepAction.performed -= OnSlipStep;
        }
        else
        {
            Debug.LogWarning("slipStepAction is NULL in OnDisable!");
        }
    }


    private void OnSlipStep(InputAction.CallbackContext context)
    {
        Debug.Log("SlipStep triggered");

        if (ComposureBar == null)
        {
            Debug.LogError("staminaBar is NULL!");
            return;
        }

        if (playerMovement == null)
        {
            Debug.LogError("playerMovement is NULL!");
            return;
        }

        if (ComposureBar.currentComposure >= ComposureCost && !playerMovement.isStaggered)
        {
            Debug.Log("Starting SlipStep!");
            StartCoroutine(SlipStepCoroutine());
        }
    }


    private IEnumerator SlipStepCoroutine()
    {
        isInvincible = true;  // Start invincibility
        playerMovement.enabled = false; // Disable normal movement

        ComposureBar.ReduceComposure(ComposureCost); // Drain stamina

        Vector3 slipDirection = new Vector3(playerMovement.moveInput.x, 0, playerMovement.moveInput.y).normalized;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + slipDirection * slipStepDistance;

        float elapsedTime = 0f;
        while (elapsedTime < slipStepDuration)
        {
            rb.MovePosition(Vector3.Lerp(startPosition, targetPosition, elapsedTime / slipStepDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPosition); // Ensure final position is exact
        isInvincible = false; // End invincibility
        playerMovement.enabled = true; // Re-enable movement
    }
}

