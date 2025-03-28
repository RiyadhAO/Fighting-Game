using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSlipStep : MonoBehaviour
{
    [Header("Slip Step Settings")]
    public float slipStepDistance = 3f;
    public float slipStepDuration = 0.15f;
    public float slipStepCooldown = 0.5f;
    public float ComposureCost = 5f;
    public bool isInvincible = false;

    private bool canSlipStep = true;
    private PlayerInput playerInput;
    private InputAction slipStepAction;
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private CombatBase combatBase; // Reference to CombatBase
    public ComposureBar ComposureBar;
    private Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        combatBase = GetComponent<CombatBase>(); // Get CombatBase
        animator = GetComponent<Animator>();

        slipStepAction = playerInput.actions["SlipStep"];

        if (slipStepAction == null)
        {
            Debug.LogError("SlipStep action not found in PlayerInput actions!");
        }
    }

    private void OnEnable()
    {
        slipStepAction.performed += OnSlipStep;
    }

    private void OnDisable()
    {
        slipStepAction.performed -= OnSlipStep;
    }

    private void OnSlipStep(InputAction.CallbackContext context)
    {
        if (ComposureBar.currentComposure <= ComposureCost + 3)
        {
            Debug.Log("Cannot step with low stamina");
            return;
        }
        // Prevent Slip Step if currently attacking
        if (canSlipStep && ComposureBar.currentComposure >= ComposureCost &&
            !playerMovement.isStaggered &&
            (combatBase == null || !combatBase.isAttacking)) // Prevent during attack
        {
            StartCoroutine(SlipStepCoroutine());
        }
        else
        {
            Debug.Log("Cannot Slip Step while attacking!");
        }
    }

    private IEnumerator SlipStepCoroutine()
    {
        canSlipStep = false;
        isInvincible = true;
        playerMovement.enabled = false;
        ComposureBar.ReduceComposure(ComposureCost);

        if (combatBase != null)
        {
            combatBase.isDefending = true; // Prevent attacks during Slip Step
        }

        Vector3 slipDirection = new Vector3(playerMovement.moveInput.x, 0, playerMovement.moveInput.y).normalized;
        Vector3 startPosition = transform.position;
        Vector3 targetPosition = startPosition + slipDirection * slipStepDistance;

        animator.SetTrigger("SlipStep");

        RaycastHit hit;
        if (Physics.Raycast(startPosition, slipDirection, out hit, slipStepDistance))
        {
            targetPosition = hit.point;
        }

        float elapsedTime = 0f;
        while (elapsedTime < slipStepDuration)
        {
            rb.MovePosition(Vector3.Lerp(startPosition, targetPosition, elapsedTime / slipStepDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.MovePosition(targetPosition);
        rb.velocity = Vector3.zero;

        isInvincible = false;
        playerMovement.enabled = true;
        animator.ResetTrigger("SlipStep");

        if (combatBase != null)
        {
            combatBase.isDefending = false; // Allow attacks again after Slip Step
        }

        yield return new WaitForSeconds(slipStepCooldown);
        canSlipStep = true;
    }
}

