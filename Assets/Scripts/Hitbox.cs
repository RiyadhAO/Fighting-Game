using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Hitbox : MonoBehaviour
{
    public enum HitboxType { Inside, Outside, Grab }
    public HitboxType hitboxType; // Publicly assignable in Inspector

    public LayerMask collisionLayers; // Assign this in Inspector to include arena boundaries

    public string attackName; // Name of the specific attack
    public float healthDamage = 10f;
    public float composureDamage = 8f;
    public string hitType;
    private AudioSource audioSource;

    [Header("Counter Hit UI (Static)")]
    public GameObject counterHitUIObject; // Assign in Inspector
    private CanvasGroup counterHitCanvasGroup;

    [Header("Counter Hit Settings")]
    public float counterHitDamage = 20f; // Counter hit bonus damage
    public AudioClip counterHitSound;
    public GameObject counterHitUIPrefab;
    public Vector3 counterHitUIOffset = new Vector3(0, 2, 0); // Slightly above the character

    [Header("Push Direction")]
    public Transform attackerPushDirectionObject;

    [Header("Grab Settings")]
    public PlayerInput attackerInput; // Use PlayerInput for new input system
    public PlayerInput targetInput;

    private Collider hitboxCollider;
    private List<Hurtbox> overlappingHurtboxes = new List<Hurtbox>();
    private bool isGrabActive = false;

    public GameObject criticalHitTextPrefab;
    public GameObject weakHitTextPrefab;

    [Header("Mash Mechanics")]
    public float maxMashTime = 1.5f;
    private int mashCount = 0;
    private InputAction mashAction;

    [Header("Knockback Settings")]
    public bool applyImpulse = false; // Toggle per attack in Inspector
    public float impulseForce = 5f; // Adjustable force magnitude
    public Vector3 impulseDirection = Vector3.forward; // Direction relative to attacker

    private List<InputAction> disabledActions = new List<InputAction>(); // Track disabled actions

    private Camera mainCamera;

    void Awake()
    {
        mainCamera = Camera.main;
    }

    void Start()
    {
        hitboxCollider = GetComponent<Collider>();
        hitboxCollider.enabled = false;

        if (attackerInput != null)
        {
            mashAction = attackerInput.actions["Mash"];
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Get CanvasGroup if available
        if (counterHitUIObject != null)
        {
            counterHitCanvasGroup = counterHitUIObject.GetComponent<CanvasGroup>();
        }
    }


    public void ActivateHitbox()
    {
        overlappingHurtboxes.Clear();
        hitboxCollider.enabled = true;
    }

    public void DeactivateHitbox()
    {
        hitboxCollider.enabled = false;
        if (!isGrabActive)
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
        if (overlappingHurtboxes.Count == 0 || hitboxType == HitboxType.Grab)
            return;

        overlappingHurtboxes.Sort((a, b) => a.priority.CompareTo(b.priority));
        Hurtbox targetHurtbox = overlappingHurtboxes[0];

        if (targetHurtbox == null || targetHurtbox.healthBar == null)
            return;

        // --- Check for Shattered Legs ---
        var injuryTracker = targetHurtbox.GetComponentInParent<CharacterInjury>();
        bool targetHasShatteredLegs = injuryTracker != null && injuryTracker.HasShatteredLegs;

        // --- Damage Calculation ---
        float damageMultiplier = 1f;
        float randomValue = Random.value;
        bool isCritical = randomValue <= 0.1f;
        bool isWeak = randomValue >= 0.9f;

        if (targetHasShatteredLegs && !isCritical && !isWeak && randomValue > 0.5f)
        {
            damageMultiplier = 1f; // Force normal hit
        }

        // Check if it's a counter hit
        CombatBase targetCombatBase = targetHurtbox.GetComponentInParent<CombatBase>();
        bool isCounterHit = targetCombatBase != null && targetCombatBase.isAttacking;

        float finalHealthDamage = (isCounterHit ? counterHitDamage : healthDamage) * damageMultiplier;
        float finalComposureDamage = composureDamage * damageMultiplier;

        if (isCounterHit)
        {
            Debug.Log("Counter Hit! Dealt " + finalHealthDamage + " damage to " + targetHurtbox.name);

            PlayCounterHitFeedback(targetHurtbox.transform.position);
        }

        // --- Apply Damage ---
        targetHurtbox.TakeDamage(finalHealthDamage, finalComposureDamage,
                               targetHurtbox.isBlockingHurtbox, hitType);

        // --- Conditional Impulse Push ---
        if (applyImpulse && !targetHurtbox.isBlockingHurtbox)
        {
            ApplyImpulse(targetHurtbox.transform, finalHealthDamage);
        }

        // --- Visual/Sound Feedback ---
        if (isCritical || isWeak)
        {
            Vector3 textPosition = targetHurtbox.transform.position + Vector3.up * 1.5f;
            SpawnFloatingText(textPosition, isCritical);
        }

        if (injuryTracker != null)
        {
            injuryTracker.RecordDamage(targetHurtbox.hurtboxType, finalHealthDamage);
        }
    }

    private void PlayCounterHitFeedback(Vector3 targetPosition)
    {
        if (counterHitSound != null && audioSource != null)
            audioSource.PlayOneShot(counterHitSound);

        if (counterHitUIPrefab != null)
        {
            Vector3 spawnPosition = targetPosition + new Vector3(8f, -1.5f, 0f);
            GameObject uiInstance = Instantiate(counterHitUIPrefab, spawnPosition, Quaternion.identity);
            StartCoroutine(AnimateCounterHitUI(uiInstance));
        }
    }



    private IEnumerator AnimateCounterHitUI(GameObject uiObject)
    {
        float duration = 1f;
        float elapsed = 0f;

        Transform uiTransform = uiObject.transform;
        Vector3 initialScale = Vector3.one;
        Vector3 targetScale = Vector3.one * 1.5f;

        CanvasGroup canvasGroup = uiObject.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = uiObject.AddComponent<CanvasGroup>();
        }
        canvasGroup.alpha = 1f;

        while (elapsed < duration)
        {
            float t = elapsed / duration;
            uiTransform.localScale = Vector3.Lerp(initialScale, targetScale, t);
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, t);

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(uiObject);
    }


    private void ApplyImpulse(Transform targetTransform, float damage)
    {
        Rigidbody targetRb = targetTransform.GetComponentInParent<Rigidbody>();
        if (targetRb == null || attackerPushDirectionObject == null)
        {
            Debug.LogWarning("Missing Rigidbody or attackerPushDirectionObject");
            return;
        }

        Vector3 pushDirection = attackerPushDirectionObject.forward;
        pushDirection.y = 0f;
        pushDirection.Normalize();

        float scaledForce = impulseForce * (1 + damage / healthDamage * 0.5f);

        Debug.Log($"Pushing {targetRb.name} in direction {pushDirection} with force {scaledForce}");

        StartCoroutine(SmoothPush(targetRb, pushDirection, scaledForce));

        PlayerState playerState = targetTransform.GetComponentInParent<PlayerState>();
        if (playerState != null)
        {
            playerState.RegisterPush();
        }
    }

    private IEnumerator SmoothPush(Rigidbody rb, Vector3 direction, float totalForce)
    {
        float duration = 0.25f;
        float timer = 0f;
        float distancePerFrame = totalForce * Time.deltaTime;

        bool wasKinematic = rb.isKinematic;
        //rb.isKinematic = false;

        while (timer < duration)
        {
            // Check if we're about to hit a wall or boundary
            if (rb.SweepTest(direction, out RaycastHit hit, distancePerFrame))
            {
                if ((collisionLayers.value & (1 << hit.collider.gameObject.layer)) != 0)
                {
                    Debug.Log("Push blocked by: " + hit.collider.name);
                    break; // Stop push if hitting arena wall or other collider
                }
            }

            rb.MovePosition(rb.position + direction * distancePerFrame);

            timer += Time.deltaTime;
            yield return null;
        }

        //rb.isKinematic = wasKinematic;
    }



    private void SpawnFloatingText(Vector3 position, bool isCritical)
    {
        GameObject prefabToSpawn = isCritical ? criticalHitTextPrefab : weakHitTextPrefab;
        if (prefabToSpawn != null)
        {
            GameObject textInstance = Instantiate(prefabToSpawn, position + new Vector3(7f, -1f, 0), Quaternion.identity);
            Destroy(textInstance, 1f);
        }
    }

    public GameObject mashPromptUI; // Assign a UI Text/Image in Inspector

    private IEnumerator ExecuteGrab(GameObject target)
    {
        isGrabActive = true;
        mashCount = 0;

        // Show mash prompt
        if (mashPromptUI != null)
            mashPromptUI.SetActive(true);

        Hurtbox hurtbox = target.GetComponent<Hurtbox>();
        if (hurtbox == null || hurtbox.isInGrab)
        {
            ResetAttacker();
            isGrabActive = false;
            yield break;
        }

        hurtbox.isInGrab = true;

        Rigidbody attackerRb = GetComponentInParent<Rigidbody>();
        Rigidbody targetRb = target.GetComponentInParent<Rigidbody>();

        bool wasAttackerKinematic = attackerRb != null && attackerRb.isKinematic;
        bool wasTargetKinematic = targetRb != null && targetRb.isKinematic;

        DisableMovementActions(attackerInput);
        DisableMovementActions(targetInput);

        if (attackerRb != null) attackerRb.isKinematic = true;
        if (targetRb != null) targetRb.isKinematic = true;

        Animator attackerAnimator = GetComponentInParent<Animator>();
        Animator targetAnimator = target.GetComponentInParent<Animator>();

        attackerAnimator?.SetTrigger("Grab");
        targetAnimator?.SetTrigger("Grabbed");

        yield return StartCoroutine(TrackMashInput());

        float baseDamage = 20f;
        float maxDamage = 50f;
        float damageMultiplier = Mathf.Clamp(1 + (mashCount / 10f), 1, maxDamage / baseDamage);
        float finalGrabDamage = baseDamage * damageMultiplier;

        hurtbox.healthBar.TakeDamage(finalGrabDamage);

        CharacterInjury injurySystem = hurtbox.GetComponentInParent<CharacterInjury>();
        if (injurySystem != null)
        {
            injurySystem.RecordDamage(hurtbox.hurtboxType, finalGrabDamage);
        }


        EnableMovementActions(attackerInput);
        EnableMovementActions(targetInput);

        if (attackerRb != null) attackerRb.isKinematic = wasAttackerKinematic;
        if (targetRb != null) targetRb.isKinematic = wasTargetKinematic;

        hurtbox.isInGrab = false;
        isGrabActive = false;

        // Hide mash prompt
        if (mashPromptUI != null)
            mashPromptUI.SetActive(false);
    }

    private IEnumerator TrackMashInput()
    {
        float timer = 0f;
        mashCount = 0;

        // Ensure action is properly set up
        if (mashAction != null)
        {
            mashAction.performed += ctx => mashCount++;
            mashAction.Enable();
        }

        while (timer < maxMashTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }

        if (mashAction != null)
        {
            mashAction.Disable();
            mashAction.performed -= ctx => mashCount++; // Unsubscribe
        }
    }

    private void DisableMovementActions(PlayerInput playerInput)
    {
        if (playerInput == null) return;

        foreach (var action in playerInput.actions)
        {
            if (action.name == "Move" || action.name == "Attack" || action.name == "Block" ||
                action.name == "SlipStep" || action.name == "EvasiveRoll" || action.name == "UseAdrenaline")
            {
                action.Disable();
                disabledActions.Add(action);
            }
        }
    }

    private void EnableMovementActions(PlayerInput playerInput)
    {
        if (playerInput == null) return;

        foreach (var action in disabledActions)
        {
            action.Enable();
        }
        disabledActions.Clear();
    }

    private void ResetAttacker()
    {
        EnableMovementActions(attackerInput);
        Rigidbody attackerRb = GetComponentInParent<Rigidbody>();
        if (attackerRb != null) attackerRb.isKinematic = false;
    }
}
