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
    private CombatBase combatScript;
    private Animator animator;
    public ComposureBar EnemyComposure;
    public TMP_Text parryText;

    private bool isTakingDamage = false; // Prevents interruption

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        combatScript = GetComponentInParent<CombatBase>();
        animator = GetComponentInParent<Animator>();

        if (parryText != null)
        {
            parryText.gameObject.SetActive(false);  // Make sure it's hidden initially
        }
    }

    public void TakeDamage(float healthDamage, float composureDamage, bool isBlocked)
    {
        if (isTakingDamage) return; // Prevent further damage animation during stun

        if (isBlocked)
        {
            if (playerMovement != null && playerMovement.isParryWindowActive)
            {
                PerformParry();
                return;
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
                StartCoroutine(PlayDamageAnimation());
            }
        }
        else
        {
            healthBar.TakeDamage(healthDamage);
            Debug.Log($"{gameObject.name} took damage! Health reduced to {healthBar.health}");
            StartCoroutine(PlayDamageAnimation());
        }
    }

    public GameObject hitEffectPrefab; // Assign in the Inspector
    public Transform hitEffectSpawnPoint; // Assign where the effect should appear

    private IEnumerator PlayDamageAnimation()
    {
        // Prevent damage animation if knockdown is happening
        if (healthBar.health <= 0) yield break;

        isTakingDamage = true;

        if (hitEffectPrefab != null && hitEffectSpawnPoint != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitEffectSpawnPoint.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 3f; // Adjust the multiplier for size
            Destroy(effect, 1.5f);
        }

        // Stop movement and attacks
        if (playerMovement != null) playerMovement.enabled = false;
        if (combatScript != null) combatScript.isAttacking = true;

        animator.SetTrigger("TakeDamage");

        // Wait for the Animator to actually change to the "TakeDamage" animation
        yield return new WaitForSeconds(0.05f); // Small delay to allow transition

        float animationDuration = 0.1f; // Default fallback

        while (animator.GetCurrentAnimatorStateInfo(0).IsTag("Damage"))
        {
            animationDuration = animator.GetCurrentAnimatorStateInfo(0).length;
            yield return null;
        }

        Debug.Log($"Playing Damage Animation, Duration: {animationDuration}");

        yield return new WaitForSeconds(animationDuration);

        // Restore movement
        isTakingDamage = false;
        if (playerMovement != null) playerMovement.enabled = true;
        if (combatScript != null) combatScript.isAttacking = false;

        Debug.Log("Player can move again!");
    }

    public GameObject parryEffectPrefab; // Assign in the Inspector
    public Transform parryEffectSpawnPoint; // Assign where the effect should appear

    private void PerformParry()
    {
        Debug.Log($"{gameObject.name} successfully parried!");
        EnemyComposure.ReduceComposure(30);

        // Show visual effect
        if (parryEffectPrefab != null && parryEffectSpawnPoint != null)
        {
            GameObject effect = Instantiate(parryEffectPrefab, parryEffectSpawnPoint.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 10f; // Adjust the multiplier for size
            Destroy(effect, 1.5f);
        }


        if (parryText != null)
        {
            StartCoroutine(ShowParryText());
        }
    }


    private IEnumerator ShowParryText()
    {
        if (parryText == null)
        {
            Debug.LogError("Parry Text is not assigned!");
            yield break;
        }

        parryText.gameObject.SetActive(true);
        yield return new WaitForSeconds(1f);
        parryText.gameObject.SetActive(false);
    }
}


