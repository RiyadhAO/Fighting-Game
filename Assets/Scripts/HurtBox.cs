using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public HealthBar healthBar;
    public StaminaBar staminaBar; // Added stamina reference
    public int priority = 2; // Lower value = higher priority
    public bool isBlockingHurtbox = false; // Distinguishes blocking hurtboxes

    public void TakeDamage(float healthDamage, float staminaDamage, bool isBlocked)
    {
        if (isBlocked)
        {
            if (staminaBar.currentStamina >= staminaDamage)
            {
                staminaBar.ReduceStamina(staminaDamage);
                Debug.Log($"{gameObject.name} blocked! Stamina reduced to {staminaBar.currentStamina}");
            }
            else
            {
                Debug.Log($"{gameObject.name} ran out of stamina! Taking health damage.");
                healthBar.TakeDamage(healthDamage); // Changed from ReduceHealth to TakeDamage
            }
        }
        else
        {
            healthBar.TakeDamage(healthDamage);
            Debug.Log($"{gameObject.name} took damage! Health reduced to {healthBar.health}");
        }
    }
}



