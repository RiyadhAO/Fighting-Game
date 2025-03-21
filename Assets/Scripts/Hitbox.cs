using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public enum HitboxType { Inside, Outside, Grab }
    public HitboxType hitboxType; // Publicly assignable in Inspector

    public string attackName; // Name of the specific attack
    public float healthDamage = 10f;
    public float composureDamage = 8f;
    public string hitType;

    [Header("Grab Settings")]
    public MonoBehaviour attackerInput; // Assign the PlayerInput component for the attacker
    public MonoBehaviour targetInput;  // Assign the PlayerInput component for the target

    private Collider hitboxCollider;
    private List<Hurtbox> overlappingHurtboxes = new List<Hurtbox>();

    private bool isGrabActive = false; // Track if a grab is currently active

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
        if (!isGrabActive) // Only deal damage if no grab is active
        {
            DealDamageToHighestPriorityHurtbox();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Hurtbox hurtbox = other.GetComponent<Hurtbox>();
        if (hurtbox != null && !overlappingHurtboxes.Contains(hurtbox))
        {
            overlappingHurtboxes.Add(hurtbox);

            // **If this is a grab attack, execute the grab sequence**
            if (hitboxType == HitboxType.Grab && !isGrabActive)
            {
                StartCoroutine(ExecuteGrab(hurtbox.gameObject));
            }
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
        if (overlappingHurtboxes.Count == 0 || hitboxType == HitboxType.Grab) return;

        overlappingHurtboxes.Sort((a, b) => a.priority.CompareTo(b.priority));
        Hurtbox targetHurtbox = overlappingHurtboxes[0];

        PlayerSlipStep slipStep = targetHurtbox.GetComponentInParent<PlayerSlipStep>();

        bool isBlocked = targetHurtbox.isBlockingHurtbox;
        bool isInvincibleToThisAttack = slipStep != null && slipStep.isInvincible && hitboxType == HitboxType.Inside;

        if (!isInvincibleToThisAttack)
        {
            targetHurtbox.TakeDamage(healthDamage, composureDamage, isBlocked, hitType);
            Debug.Log($"{attackName} hit {targetHurtbox.gameObject.name}!");
        }
        else
        {
            Debug.Log($"{attackName} was avoided by Slip Step!");
        }
    }

    private IEnumerator ExecuteGrab(GameObject target)
    {
        isGrabActive = true; // Mark grab as active

        Hurtbox hurtbox = target.GetComponent<Hurtbox>();
        if (hurtbox == null || hurtbox.isInGrab)
        {
            Debug.Log("Grab attempt failed: No valid target.");
            ResetAttacker();  // Ensure attacker can move and attack again
            isGrabActive = false; // Mark grab as inactive
            yield break;
        }

        hurtbox.isInGrab = true;

        Debug.Log($"{target.name} was grabbed by {gameObject.name} using {attackName}!");

        // Get attacker and target components
        Rigidbody attackerRb = GetComponentInParent<Rigidbody>();
        Rigidbody targetRb = target.GetComponentInParent<Rigidbody>();

        // **Disable both players' input actions**
        if (attackerInput != null) attackerInput.enabled = false;
        if (targetInput != null) targetInput.enabled = false;

        // **Freeze both players' rigid bodies**
        if (attackerRb != null)
        {
            attackerRb.isKinematic = true; // Freeze the rigidbody
        }
        if (targetRb != null)
        {
            targetRb.isKinematic = true; // Freeze the rigidbody
        }

        // Play grab animations
        Animator attackerAnimator = GetComponentInParent<Animator>();
        Animator targetAnimator = target.GetComponentInParent<Animator>();

        if (attackerAnimator != null) attackerAnimator.SetTrigger("Grab");
        if (targetAnimator != null) targetAnimator.SetTrigger("Grabbed");

        yield return new WaitForSeconds(1.5f); // Wait for grab animation to complete

        // Apply damage only if the grab connected
        if (hurtbox != null)
        {
            hurtbox.healthBar.TakeDamage(40);
            Debug.Log($"{target.name} took grab damage from {attackName}!");
        }

        // **Re-enable both players' input actions and unfreeze rigid bodies**
        ResetAttacker();
        if (targetInput != null) targetInput.enabled = true;
        if (targetRb != null) targetRb.isKinematic = false;

        if (attackerInput != null)
        {
            attackerInput.enabled = true;
        }
        if (attackerRb != null)
        {
            attackerRb.isKinematic = false;
        }

        hurtbox.isInGrab = false;
        isGrabActive = false; // Mark grab as inactive
    }

    // **New Function**: Ensures the attacker regains control after grab sequence
    private void ResetAttacker()
    {
        if (attackerInput != null) attackerInput.enabled = true;

        Rigidbody attackerRb = GetComponentInParent<Rigidbody>();
        if (attackerRb != null) attackerRb.isKinematic = false;

        Debug.Log("Attacker can move and attack again!");
    }
}