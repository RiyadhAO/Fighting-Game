using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerEvasiveRoll : MonoBehaviour
{
    [Header("Evasive Roll Settings")]
    public float rollForce = 10f;          // How strong the roll is
    public float rollDuration = 0.5f;      // How long the roll lasts
    public float rollCooldown = 1f;        // Cooldown before rolling again
    public float composureCost = 10f;      // Energy cost for rolling
    public bool isInvincible = false;      // Temporarily immune during roll
    public LayerMask collisionLayers;      // Layers the player can collide with

    private PlayerInput playerInput;
    private InputAction rollAction;
    private Rigidbody rb;
    private PlayerMovement playerMovement;
    private CombatBase combatBase; // Reference to CombatBase to check for attacks
    public ComposureBar ComposureBar;
    private Animator animator;

    private Vector3 rollDirection;
    public bool isRolling = false;
    private bool canRoll = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        playerMovement = GetComponent<PlayerMovement>();
        combatBase = GetComponent<CombatBase>(); // Get CombatBase reference
        animator = GetComponent<Animator>();

        if (playerMovement == null)
            Debug.LogError("PlayerMovement is NULL in PlayerEvasiveRoll!");

        if (animator == null)
            Debug.LogError("Animator is NULL in PlayerEvasiveRoll!");

        rollAction = playerInput.actions["EvasiveRoll"];

        if (rollAction == null)
            Debug.LogError("EvasiveRoll action not found in PlayerInput actions!");
    }

    private void OnEnable()
    {
        rollAction.performed += OnEvasiveRoll;
    }

    private void OnDisable()
    {
        if (rollAction != null)
            rollAction.performed -= OnEvasiveRoll;
    }

    private void OnEvasiveRoll(InputAction.CallbackContext context)
    {
        if (ComposureBar == null || playerMovement == null || isRolling || !canRoll) return;

        // Prevent rolling if attacking
        if (combatBase != null && combatBase.isAttacking)
        {
            Debug.Log("Cannot roll while attacking!");
            return;
        }

        if (ComposureBar.currentComposure <= composureCost + 5)
        {
            Debug.Log("Cannot roll with low stamina");
            return;
        }

        if (ComposureBar.currentComposure >= composureCost && !playerMovement.isStaggered)
        {
            rollDirection = new Vector3(playerMovement.moveInput.x, 0, playerMovement.moveInput.y).normalized;

            if (rollDirection == Vector3.zero) return; // Prevent rolling in place

            StartCoroutine(EvasiveRollCoroutine());
        }
    }

    private IEnumerator EvasiveRollCoroutine()
    {
        isRolling = true;
        isInvincible = true;
        canRoll = false; // Prevent rolling again immediately
        ComposureBar.ReduceComposure(composureCost);

        if (combatBase != null)
        {
            combatBase.isDefending = true; // Prevent attacks during roll
        }

        rb.useGravity = false;
        rb.velocity = Vector3.zero;

        // Play roll animation
        animator.SetFloat("RollX", rollDirection.x);
        animator.SetFloat("RollZ", rollDirection.z);
        animator.SetTrigger("Roll");

        float elapsedTime = 0f;

        while (elapsedTime < rollDuration)
        {
            // **Collision Check Before Moving**
            if (rb.SweepTest(rollDirection, out RaycastHit hit, rollForce * Time.deltaTime))
            {
                Debug.Log("Roll blocked by: " + hit.collider.name);
                break; // Stop movement if a wall is detected
            }

            // Apply force-based movement
            rb.AddForce(rollDirection * rollForce, ForceMode.VelocityChange);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        rb.useGravity = true;
        isInvincible = false;
        isRolling = false;

        if (combatBase != null)
        {
            combatBase.isDefending = false; // Allow attacking again after roll
        }

        // Cooldown before rolling again
        yield return new WaitForSeconds(rollCooldown);
        canRoll = true;
    }
}
