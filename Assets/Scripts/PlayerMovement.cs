using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 5f;
    public float blockSpeedMultiplier = 0.5f;
    public Transform enemy;

    private Vector2 moveInput;
    private Rigidbody rb;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction blockAction;

    private bool isBlocking = false;
    private Hurtbox[] normalHurtboxes;
    private Hurtbox[] blockingHurtboxes;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        blockAction = playerInput.actions["Block"];

        Hurtbox[] allHurtboxes = GetComponentsInChildren<Hurtbox>();
        normalHurtboxes = System.Array.FindAll(allHurtboxes, hb => !hb.isBlockingHurtbox);
        blockingHurtboxes = System.Array.FindAll(allHurtboxes, hb => hb.isBlockingHurtbox);
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
        moveInput = context.ReadValue<Vector2>();
    }

    private void OnBlockStart(InputAction.CallbackContext context)
    {
        isBlocking = true;
        SetHurtboxState(normalHurtboxes, false);
        SetHurtboxState(blockingHurtboxes, true);
    }

    private void OnBlockEnd(InputAction.CallbackContext context)
    {
        isBlocking = false;
        SetHurtboxState(normalHurtboxes, true);
        SetHurtboxState(blockingHurtboxes, false);
    }

    private void SetHurtboxState(Hurtbox[] hurtboxes, bool state)
    {
        foreach (var hb in hurtboxes)
        {
            hb.gameObject.SetActive(state);
        }
    }

    private void FixedUpdate()
    {
        if (enemy == null) return;

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
}



