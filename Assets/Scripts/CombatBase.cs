using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class CombatBase : MonoBehaviour
{
    protected bool isCharging = false;
    protected float chargeTime = 0.01f;
    public Animator animator;
    public bool isAttacking = false;

    // New: Add isDefending to prevent attacking while dodging or blocking
    public bool isDefending = false;

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
        if (isDefending)
        {
            Debug.Log("Cannot attack while defending!");
            return; // Prevent attacking if in a defensive state
        }

        string inputActionName = context.control.name;
        Debug.Log($"Attack Input Received: {inputActionName}");

        if (AttackMoves.ContainsKey(inputActionName) && !isAttacking)
        {
            string attackName = AttackMoves[inputActionName];
            StartAttack(attackName);
        }
    }

    protected void StartAttack(string attackName)
    {
        if (isAttacking) return;

        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.IsName("TakeDamage"))
        {
            Debug.Log("Attack interrupted by damage!");
            return;
        }

        if (!attackHitboxes.ContainsKey(attackName))
        {
            Debug.LogWarning($"No hitbox found for attack: {attackName}");
            return;
        }

        Debug.Log($"Starting attack: {attackName}");
        isAttacking = true;
        animator.SetTrigger(attackName);

        StartCoroutine(AttackDuration(attackName));
    }

    private IEnumerator AttackDuration(string attackName)
    {
        yield return null;

        while (true)
        {
            AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("TakeDamage"))
            {
                Debug.Log("Attack interrupted by damage!");
                isAttacking = false;
                yield break;
            }

            if (stateInfo.IsName(attackName) && stateInfo.normalizedTime >= 1.0f)
            {
                break;
            }

            yield return null;
        }

        isAttacking = false;
    }

    public void ActivateHitbox(string attackName)
    {
        if (attackHitboxes.ContainsKey(attackName))
        {
            attackHitboxes[attackName].ActivateHitbox();
            Debug.Log("Hitbox activated");
        }
    }

    public void DeactivateHitbox(string attackName)
    {
        if (attackHitboxes.ContainsKey(attackName))
        {
            attackHitboxes[attackName].DeactivateHitbox();
        }
    }

    public void ResetAttackState()
    {
        isAttacking = false;
    }
}
