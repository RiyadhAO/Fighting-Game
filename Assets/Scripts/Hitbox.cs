using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    public float healthDamage = 10f; // Health damage amount
    public float staminaDamage = 15f; // Stamina drain amount
    public string attackName;

    private Collider hitboxCollider;
    private List<Hurtbox> overlappingHurtboxes = new List<Hurtbox>();
    private CombatBase ownerCombat;

    void Start()
    {
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = false;
        ownerCombat = GetComponentInParent<CombatBase>(); // Get the attacker's script
    }

    public void ActivateHitbox()
    {
        overlappingHurtboxes.Clear();
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
            if (hurtbox.GetComponentInParent<CombatBase>() != ownerCombat) // Avoid hitting self
            {
                overlappingHurtboxes.Add(hurtbox);
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
        if (overlappingHurtboxes.Count == 0) return;

        List<Hurtbox> blockingHurtboxes = overlappingHurtboxes.FindAll(hb => hb.isBlockingHurtbox);
        List<Hurtbox> normalHurtboxes = overlappingHurtboxes.FindAll(hb => !hb.isBlockingHurtbox);

        Hurtbox chosenHurtbox = blockingHurtboxes.Count > 0 ? blockingHurtboxes[0] : normalHurtboxes[0];

        if (chosenHurtbox != null)
        {
            bool isBlocked = blockingHurtboxes.Count > 0;
            chosenHurtbox.TakeDamage(healthDamage, staminaDamage, isBlocked);
        }
    }
}



