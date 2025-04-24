using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Hurtbox : MonoBehaviour
{
    public HurtboxType hurtboxType; // Set in Inspector

    public HealthBar healthBar;
    public ComposureBar composureBar;
    public Transform knockbackAnchor; // Drag in the knockback direction anchor on prefab
    public int priority = 2; // Lower value = higher priority
    public bool isBlockingHurtbox = false; // Distinguishes blocking hurtboxes
    private PlayerMovement playerMovement;
    private CombatBase combatScript;
    private Animator animator;
    public ComposureBar EnemyComposure;
    private UseAdrenaline useAdrenaline; // Added reference to UseAdrenaline

    public bool isInGrab = false; // Prevents multiple grabs
    private bool isTakingDamage = false; // Prevents interruption

    public AudioSource audioSource; // Audio source for playing sounds
    public AudioClip lightHitSound;
    public AudioClip heavyHitSound;
    public AudioClip blockSound;
    public AudioClip parrySound; // New parry sound effect

    private void Start()
    {
        playerMovement = GetComponentInParent<PlayerMovement>();
        combatScript = GetComponentInParent<CombatBase>();
        animator = GetComponentInParent<Animator>();
        useAdrenaline = GetComponentInParent<UseAdrenaline>(); // Initialize UseAdrenaline

        if (useAdrenaline == null)
        {
            Debug.LogError("UseAdrenaline component missing on " + gameObject.name);
        }
    }

    public void TakeDamage(float healthDamage, float composureDamage, bool isBlocked, string hitType)
    {
        if (isTakingDamage || isInGrab) return; // Prevents damage during grabs or stun

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
                PlaySound(blockSound);
                Debug.Log($"{gameObject.name} blocked! Stamina reduced to {composureBar.currentComposure}");
            }
            else
            {
                Debug.Log($"{gameObject.name} ran out of stamina! Taking health damage.");
                composureBar.currentComposure = 0;
                PlayHitSound(hitType);
                playerMovement.BreakGuard();
                StartCoroutine(PlayDamageAnimation());
            }
        }
        else
        {
            healthBar.TakeDamage(healthDamage);
            PlayHitSound(hitType);
            Debug.Log($"{gameObject.name} took damage! Health reduced to {healthBar.health}");
            StartCoroutine(PlayDamageAnimation());
        }
    }

    public GameObject hitEffectPrefab;
    public Transform hitEffectSpawnPoint;

    private IEnumerator PlayDamageAnimation()
    {
        if (healthBar.health <= 0) yield break;

        // **Skip damage animation if adrenaline is active**
        if (useAdrenaline != null && useAdrenaline.isAdrenalineActive)
        {
            Debug.Log("Adrenaline active! Skipping damage animation.");
            yield break;
        }

        isTakingDamage = true;

        if (hitEffectPrefab != null && hitEffectSpawnPoint != null)
        {
            GameObject effect = Instantiate(hitEffectPrefab, hitEffectSpawnPoint.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 3f;
            Destroy(effect, 1.5f);
    }

        if (playerMovement != null) playerMovement.enabled = false;
        if (combatScript != null) combatScript.isAttacking = true;

        animator.SetTrigger("TakeDamage");

        yield return new WaitForSeconds(0.05f);

        float animationDuration = 0.1f;
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("Damage"))
        {
            animationDuration = stateInfo.length;
        }

        Debug.Log($"Playing Damage Animation, Duration: {animationDuration}");
        yield return new WaitForSeconds(animationDuration);

        isTakingDamage = false;
        if (playerMovement != null) playerMovement.enabled = true;
        if (combatScript != null) combatScript.isAttacking = false;
    }

    public GameObject parryEffectPrefab;
    public Transform parryEffectSpawnPoint;

    private void PerformParry()
    {
        Debug.Log($"{gameObject.name} successfully parried!");
        EnemyComposure.ReduceComposure(30);

        if (parryEffectPrefab != null && parryEffectSpawnPoint != null)
        {
            GameObject effect = Instantiate(parryEffectPrefab, parryEffectSpawnPoint.position, Quaternion.identity);
            effect.transform.localScale = Vector3.one * 10f;
            Destroy(effect, 1.5f);
        }

        PlaySound(parrySound); // Play parry sound effect
    }

    private void PlayHitSound(string hitType)
    {
        switch (hitType)
        {
            case "light":
                PlaySound(lightHitSound);
                break;
            case "heavy":
                PlaySound(heavyHitSound);
                break;
            default:
                Debug.LogWarning("Unknown hitbox type! No sound played.");
                break;
        }
    }

    private void PlaySound(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }
}
