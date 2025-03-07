using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hurtbox : MonoBehaviour
{
    public HealthBar healthBar;
    public ComposureBar composureBar;
    public int priority = 2; // Lower value = higher priority
    public bool isBlockingHurtbox = false; // Distinguishes blocking hurtboxes
    private PlayerMovement playerMovement;
    public ComposureBar EnemyComposure;
    public TMP_Text parryText;  // Assign this in the Inspector

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        if (parryText != null)
        {
            parryText.gameObject.SetActive(false);  // Make sure it's hidden initially
        }
    }

    public void TakeDamage(float healthDamage, float composureDamage, bool isBlocked)
    {
        if (isBlocked)
        {
            // Check if the player is in the parry window
            if (playerMovement != null && playerMovement.isParryWindowActive)
            {
                PerformParry();
                return;  // Do not proceed with normal blocking behavior
            }

            if (composureBar.currentComposure >= composureDamage)
            {
                composureBar.ReduceComposure(composureDamage);
                Debug.Log($"{gameObject.name} blocked! Stamina reduced to {composureBar.currentComposure}");
            }
            else
            {
                Debug.Log($"{gameObject.name} ran out of stamina! Taking health damage.");
                healthBar.TakeDamage(healthDamage);
            }
        }
        else
        {
            healthBar.TakeDamage(healthDamage);
            Debug.Log($"{gameObject.name} took damage! Health reduced to {healthBar.health}");
        }
    }

    private void PerformParry()
    {
        Debug.Log($"{gameObject.name} successfully parried!");

        EnemyComposure.ReduceComposure(30);

        if (parryText != null)
        {
            Debug.Log("Triggering ShowParryText Coroutine");  // Debugging message
            StartCoroutine(ShowParryText());
        }
    }


    private IEnumerator ShowParryText()
    {
        if (parryText == null)
        {
            Debug.LogError("Parry Text is not assigned!");
            yield break;  // Exit the coroutine if there's no text assigned
        }

        parryText.gameObject.SetActive(true);  // Show text
        yield return new WaitForSeconds(1f);   // Wait for 1 second
        parryText.gameObject.SetActive(false); // Hide text
    }

}



