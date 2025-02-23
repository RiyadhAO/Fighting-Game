using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatBase : MonoBehaviour
{
    public Animator animator;
    protected bool isAttacking = false;
    public Hitbox hitbox;

    // Dictionary mapping key inputs to attack names
    protected abstract Dictionary<KeyCode, string> AttackMoves { get; }

    protected virtual void Update()
    {
        foreach (var move in AttackMoves)
        {
            if (Input.GetKeyDown(move.Key) && !isAttacking)
            {
                PerformAttack(move.Value);
            }
        }
    }

    protected void PerformAttack(string attackName)
    {
        isAttacking = true;
        animator.SetTrigger(attackName);
        hitbox.ActivateHitbox(); // Enable hitbox for attack
        StartCoroutine(ResetAttack());
    }

    protected IEnumerator ResetAttack()
    {
        yield return new WaitForSeconds(0.2f); // Ensures quick response time
        isAttacking = false;
    }
}
