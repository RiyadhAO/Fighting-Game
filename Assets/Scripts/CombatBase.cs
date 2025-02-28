using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatBase : MonoBehaviour
{
    public Animator animator;
    protected bool isAttacking = false;

    // Dictionary of attack names linked to hitboxes
    public Dictionary<string, Hitbox> attackHitboxes = new Dictionary<string, Hitbox>();

    protected abstract Dictionary<KeyCode, string> AttackMoves { get; }

    protected virtual void Start()
    {
        // Automatically detect and store hitboxes for each move
        Hitbox[] hitboxComponents = GetComponentsInChildren<Hitbox>();
        foreach (Hitbox hitbox in hitboxComponents)
        {
            attackHitboxes[hitbox.attackName] = hitbox;
        }
    }

    protected virtual void Update()
    {
        foreach (var move in AttackMoves)
        {
            if (Input.GetKeyDown(move.Key) && !isAttacking)
            {
                StartAttack(move.Value);
            }
        }
    }

    protected void StartAttack(string attackName)
    {
        if (!attackHitboxes.ContainsKey(attackName)) return;

        isAttacking = true;
        animator.SetTrigger(attackName);
    }

    // Called by animation events at the right frame
    public void ActivateHitbox(string attackName)
    {
        if (attackHitboxes.ContainsKey(attackName))
        {
            attackHitboxes[attackName].ActivateHitbox();
        }
    }

    // Called by animation events to turn off hitbox
    public void DeactivateHitbox(string attackName)
    {
        if (attackHitboxes.ContainsKey(attackName))
        {
            attackHitboxes[attackName].DeactivateHitbox();
        }
        isAttacking = false; // Allow next attack
    }
}


