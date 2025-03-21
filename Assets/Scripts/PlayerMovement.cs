using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float blockSpeedMultiplier = 0.5f;
    public Transform enemy;

    public Vector2 moveInput { get; private set; }
    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction blockAction;

    private bool isBlocking = false;
    public bool isStaggered = false;
    private Animator animator;

    private InputAction attackAction;
    private InputAction rollAction;
    private InputAction slipStepAction;

    private Hurtbox[] normalHurtboxes;
    private Hurtbox[] blockingHurtboxes;
    public ComposureBar composureBar;  // Ensure this is assigned in the inspector

    public bool isParryWindowActive = false;
    public float parryWindowDuration = 0.3f;  // Time frame for a parry

    private CombatBase combatBase;
    private PlayerEvasiveRoll evasiveRoll;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();

        combatBase = GetComponent<CombatBase>(); // Reference to CombatBase for attack checks
        evasiveRoll = GetComponent<PlayerEvasiveRoll>(); // Reference to Evasive Roll script

        attackAction = playerInput.actions["Attack"];
        rollAction = playerInput.actions["EvasiveRoll"];
        slipStepAction = playerInput.actions["SlipStep"];

        // If composureBar is not assigned, log an error
        if (composureBar == null)
        {
            Debug.LogError("ComposureBar component is not assigned! Please assign it in the inspector.");
        }

        moveAction = playerInput.actions["Move"];
        blockAction = playerInput.actions["Block"];

        Hurtbox[] allHurtboxes = GetComponentsInChildren<Hurtbox>();
        normalHurtboxes = System.Array.FindAll(allHurtboxes, hb => !hb.isBlockingHurtbox);
        blockingHurtboxes = System.Array.FindAll(allHurtboxes, hb => hb.isBlockingHurtbox);

        // Disable blocking hurtboxes at the start
        SetHurtboxState(blockingHurtboxes, false);
    }

    private void OnEnable()
    {
        moveAction.performed += OnMove;
        moveAction.canceled += OnMove;
        blockAction.performed += OnBlockStart;
        blockAction.canceled += OnBlockEnd;
    }

    private void OnDisable()
    {
        moveAction.performed -= OnMove;
        moveAction.canceled -= OnMove;
        blockAction.performed -= OnBlockStart;
        blockAction.canceled -= OnBlockEnd;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (isStaggered) return; // Prevent movement during stagger
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnBlockStart(InputAction.CallbackContext context)
    {
        if (isStaggered || composureBar.currentComposure <= 0)
        {
            Debug.Log("Cannot block due to stagger or low composure!");
            return;
        }

        // Prevent blocking if currently attacking, rolling, or slip stepping
        if ((combatBase != null && combatBase.isAttacking) || (evasiveRoll != null && evasiveRoll.isRolling))
        {
            Debug.Log("Cannot block while attacking or rolling!");
            return;
        }

        isBlocking = true;
        SetHurtboxState(normalHurtboxes, false);
        SetHurtboxState(blockingHurtboxes, true);

        // **Play blocking animation**
        animator.SetBool("IsBlocking", true);

        // **Disable attack, evade, and slip step actions**
        attackAction.Disable();
        rollAction.Disable();
        slipStepAction.Disable();

        // Start the parry window
        StartCoroutine(ParryWindow());

        Debug.Log("Player is blocking, attack and evade disabled.");
    }

    public void OnBlockEnd(InputAction.CallbackContext context)
    {
        if (isStaggered)
        {
            Debug.Log("Player is staggered, cannot stop blocking!");
            return;
        }

        isBlocking = false;
        SetHurtboxState(normalHurtboxes, true);
        SetHurtboxState(blockingHurtboxes, false);

        // **Stop blocking animation**
        animator.SetBool("IsBlocking", false);

        // **Re-enable attack, evade, and slip step actions**
        attackAction.Enable();
        rollAction.Enable();
        slipStepAction.Enable();

        Debug.Log("Player stopped blocking, attack and evade re-enabled.");
    }

    private void SetHurtboxState(Hurtbox[] hurtboxes, bool state)
    {
        foreach (var hb in hurtboxes)
        {
            hb.gameObject.SetActive(state);
        }
    }

    public void BreakGuard()
    {
        OnBlockEnd(new InputAction.CallbackContext());
    }

    private void Update()
    {
        // Check stagger state every frame
        CheckStaggerState();

        // Force block to stop if composure reaches zero
        if (isBlocking && composureBar.currentComposure <= 2)
        {
            Debug.Log("Composure depleted! Forcing block to end.");
            OnBlockEnd(new InputAction.CallbackContext());
        }
    }

    private void FixedUpdate()
    {
        if (enemy == null) return;

        // Maintain rotation towards the enemy
        Vector3 direction = enemy.position - transform.position;
        direction.y = 0; // Keep the rotation horizontal (no tilting up/down)

        if (direction != Vector3.zero)
        {
            float rotationSpeed = 20f; // Adjust for faster/slower turning
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90, 0);
            rb.MoveRotation(Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime));
        }

        // Only move if not staggered
        if (!isStaggered)
        {
            float currentSpeed = isBlocking ? speed * blockSpeedMultiplier : speed;
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * currentSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }


    private void CheckStaggerState()
    {
        if (composureBar == null) return;

        if (!isStaggered && composureBar.currentComposure <= 2)
        {
            StartCoroutine(StaggerEffect());
            Debug.LogWarning("Player staggered!");
        }
    }

    private IEnumerator ParryWindow()
    {
        isParryWindowActive = true;
        Debug.Log("Parry window open!");

        yield return new WaitForSeconds(parryWindowDuration);

        isParryWindowActive = false;
        Debug.Log("Parry window closed.");
    }

    private IEnumerator StaggerEffect()
    {
        animator.SetTrigger("Stagger");

        isStaggered = true;
        isBlocking = false;

        SetHurtboxState(normalHurtboxes, true);
        SetHurtboxState(blockingHurtboxes, false);

        Debug.Log("Player is staggered!");

        playerInput.enabled = false;

        while (composureBar.currentComposure < composureBar.maxComposure)
        {
            composureBar.regenRate = 30;
            yield return null;
        }

        composureBar.regenRate = 3;
        isStaggered = false;
        Debug.Log("Player recovered from stagger.");

        playerInput.enabled = true;
    }
}

