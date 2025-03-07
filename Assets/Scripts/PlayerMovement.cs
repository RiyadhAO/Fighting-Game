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

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();
        animator = GetComponent<Animator>();  // Reference to the Animator component
        attackAction = playerInput.actions["Attack"];  // Replace "Attack" with the actual action name
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

        isBlocking = true;
        SetHurtboxState(normalHurtboxes, false);
        SetHurtboxState(blockingHurtboxes, true);

        // **Play blocking animation**
        animator.SetBool("IsBlocking", true);

        // **Disable attack and evade actions**
        attackAction.Disable();
        rollAction.Disable();
        slipStepAction.Disable();

        // Start the parry window
        StartCoroutine(ParryWindow());

        Debug.Log("Player is blocking, attack and evade disabled.");
    }


    private void OnBlockEnd(InputAction.CallbackContext context)
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

        // **Re-enable attack and evade actions**
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

    private void Update()
    {
        // Check stagger state every frame (Update is called more frequently than FixedUpdate)
        CheckStaggerState();
    }

    private void FixedUpdate()
    {
        if (enemy == null || isStaggered) return;

        float currentSpeed = isBlocking ? speed * blockSpeedMultiplier : speed;
        Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y) * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        Vector3 direction = enemy.position - transform.position;
        direction.y = 0;

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -90, 0);
            rb.MoveRotation(targetRotation);
        }
    }

    private void CheckStaggerState()
    {
        if (composureBar == null) return; // Prevent null reference error

        if (!isStaggered && composureBar.currentComposure <= 0)
        {
            // If composure reaches 0, trigger stagger effect
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
        // Trigger stagger animation
        animator.SetTrigger("Stagger");

        // Disable movement, blocking, and attacking
        isStaggered = true;
        isBlocking = false;

        SetHurtboxState(normalHurtboxes, true);
        SetHurtboxState(blockingHurtboxes, false);

        Debug.Log("Player is staggered!");

        // Disable player movement during stagger
        moveInput = Vector2.zero; // Disable movement input

        // Wait until composure is fully regenerated (i.e., 100%)
        while (composureBar.currentComposure < composureBar.maxComposure)
        {
            yield return null; // Keep waiting while composure is regenerating
        }

        // Player has fully recovered from stagger when composure reaches 100%
        isStaggered = false;
        Debug.Log("Player recovered from stagger.");

        // Optionally, resume movement once stagger is over
        moveInput = Vector2.zero; // Player remains still until stagger ends
    }
}
