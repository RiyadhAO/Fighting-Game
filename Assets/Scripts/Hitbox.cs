using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public enum HitboxType { Inside, Outside }  // Defines Inside and Outside attacks
    public HitboxType hitboxType; // Publicly assignable in Inspector

    public float healthDamage = 10f;  // Damage dealt to health
    public float composureDamage = 8f;  // Damage dealt to stamina when blocking
    public string attackName; // Specifies which move this hitbox is for

    private Collider hitboxCollider;
    private List<Hurtbox> overlappingHurtboxes = new List<Hurtbox>();

    void Start()
    {
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = false;
    }

    public void ActivateHitbox()
    {
        overlappingHurtboxes.Clear(); // Reset previous collisions
        hitboxCollider.enabled = true;
    }

    public void DeactivateHitbox()
    {
        hitboxCollider.enabled = false;
        DealDamageToHighestPriorityHurtbox();
    }

    private void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null && !overlappingHurtboxes.Contains(hurtbox))
        {
            overlappingHurtboxes.Add(hurtbox);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null && overlappingHurtboxes.Contains(hurtbox))
        {
            overlappingHurtboxes.Remove(hurtbox);
        }
    }

    private void DealDamageToHighestPriorityHurtbox()
    {
        if (overlappingHurtboxes.Count == 0) return;

        // Sort hurtboxes by priority (lower number = higher priority)
        overlappingHurtboxes.Sort((a, b) => a.priority.CompareTo(b.priority));
        Hurtbox targetHurtbox = overlappingHurtboxes[0];

        // Get the PlayerSlipStep component from the hurtbox owner (if applicable)
        PlayerSlipStep slipStep = targetHurtbox.GetComponentInParent<PlayerSlipStep>();

        // Determine if attack should be negated due to Slip Step
        bool isBlocked = targetHurtbox.isBlockingHurtbox;
        bool isInvincibleToThisAttack = slipStep != null && slipStep.isInvincible && hitboxType == HitboxType.Inside;

        // Only apply damage if the attack is NOT negated by Slip Step
        if (!isInvincibleToThisAttack)
        {
            targetHurtbox.TakeDamage(healthDamage, composureDamage, isBlocked);
        }
        else
        {
            Debug.Log($"{attackName} was avoided by Slip Step!");
        }
    }
}




