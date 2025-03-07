using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CombatBase : MonoBehaviour
{
    protected bool isCharging = false;
    protected float chargeTime = 1.5f; // Adjust charge time as needed

    public Animator animator;
    protected bool isAttacking = false;

    // Dictionary of attack names linked to hitboxes
    public Dictionary<string, Hitbox> attackHitboxes = new Dictionary<string, Hitbox>();

    protected abstract Dictionary<string, string> AttackMoves { get; }
    protected PlayerInput playerInput;
    private InputAction attackAction;

    protected virtual void Start()
    {
        Hitbox[] hitboxComponents = GetComponentsInChildren<Hitbox>();

        if (hitboxComponents.Length == 0)
        {
            Debug.LogError($"No Hitbox components found on {gameObject.name}");
        }

        foreach (Hitbox hitbox in hitboxComponents)
        {
            attackHitboxes[hitbox.attackName] = hitbox;
            Debug.Log($"Registered hitbox: {hitbox.attackName}");
        }
    }


    protected virtual void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("PlayerInput component missing on " + gameObject.name);
            return;
        }

        attackAction = playerInput.actions["Attack"];
        if (attackAction == null)
        {
            Debug.LogError("Attack action not found in Input Actions Asset");
        }
    }

    protected virtual void OnEnable()
    {
        if (attackAction != null)
            attackAction.performed += OnAttackPerformed;
    }

    protected virtual void OnDisable()
    {
        if (attackAction != null)
            attackAction.performed -= OnAttackPerformed;
    }

    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        string inputActionName = context.control.name;

        Debug.Log($"Attack Input Received: {inputActionName}");

        if (AttackMoves.ContainsKey(inputActionName) && !isAttacking)
        {
            string attackName = AttackMoves[inputActionName];

            if (attackName == "Tackle")
            {
                StartCoroutine(ChargeAndTackle(attackName));
            }
            else
            {
                StartAttack(attackName);
            }
        }
    }

    private IEnumerator ChargeAndTackle(string attackName)
    {
        isCharging = true;
        Debug.Log("Charging tackle...");

        // Play charge-up animation
        animator.SetTrigger("Charge");

        yield return new WaitForSeconds(chargeTime); // Wait for the charge duration

        isCharging = false;

        // Transition from charge to tackle
        animator.SetTrigger(attackName);
        StartCoroutine(HandleGrab());
    }


    protected void StartAttack(string attackName)
    {
        if (!attackHitboxes.ContainsKey(attackName))
        {
            Debug.LogWarning($"No hitbox found for attack: {attackName}");
            return;
        }

        Debug.Log($"Starting attack: {attackName}");
        isAttacking = true;
        animator.SetTrigger(attackName);

        if (attackName == "Tackle")
        {
            StartCoroutine(HandleGrab());
        }
    }

    private IEnumerator HandleGrab()
    {
        yield return new WaitForSeconds(0.5f); // Simulate grab timing

        GameObject enemy = DetectEnemy();
        if (enemy != null)
        {
            Debug.Log("Enemy grabbed! Initiating joint animation...");
            animator.SetTrigger("Grab");
            enemy.GetComponent<Animator>().SetTrigger("Grabbed");
        }

        yield return new WaitForSeconds(1.5f); // Duration of grab animation

        isAttacking = false;
    }

    private GameObject DetectEnemy()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, 2f);
        foreach (Collider col in hitColliders)
        {
            if (col.CompareTag("Enemy"))
            {
                return col.gameObject;
            }
        }
        return null;
    }


    public void ActivateHitbox(string attackName)
    {
        if (attackHitboxes.ContainsKey(attackName))
        {
            attackHitboxes[attackName].ActivateHitbox();
            Debug.Log("hitbox activated");
        }
    }

    public void DeactivateHitbox(string attackName)
    {
        if (attackHitboxes.ContainsKey(attackName))
        {
            attackHitboxes[attackName].DeactivateHitbox();
        }
        isAttacking = false;
    }
}
