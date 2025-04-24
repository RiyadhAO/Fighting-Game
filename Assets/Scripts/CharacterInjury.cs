using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CharacterInjury : MonoBehaviour
{
    public HurtboxType hurtboxType;

    [Header("Injury UI")]
    public TextMeshProUGUI injuryText; // Direct reference to a TMP text element
    public float messageDuration = 3f;
    public float fadeDuration = 0.5f;

    [Header("Health Reference")]
    public HealthBar healthBar;

    private Dictionary<HurtboxType, float> damageMap = new Dictionary<HurtboxType, float>();
    private bool hasBrokenRibs = false;
    private bool hasShatteredLegs = false;
    private bool hasHaze = false;
    private int brokenRibsStack = 0;
    private bool adrenalineActive = false;
    private Coroutine currentMessageCoroutine;

    void Awake()
    {
        // Initialize all damage tracking
        foreach (HurtboxType type in System.Enum.GetValues(typeof(HurtboxType)))
        {
            damageMap[type] = 0f;
        }

        // Hide injury text at start
        if (injuryText != null)
        {
            injuryText.text = "";
            injuryText.gameObject.SetActive(false);
        }
    }

    public void RecordDamage(HurtboxType type, float amount)
    {
        damageMap[type] += amount;
    }

    public void EvaluateInjuryAfterKnockdown()
    {
        // Don't trigger if no damage
        if (damageMap.Count == 0) return;

        HurtboxType worst = HurtboxType.Body;
        float highest = 0f;

        foreach (var entry in damageMap)
        {
            if (entry.Value > highest)
            {
                highest = entry.Value;
                worst = entry.Key;
            }
        }

        bool applyInjury = Random.value < 0.5f;
        if (!applyInjury) return;

        switch (worst)
        {
            case HurtboxType.Body:
                brokenRibsStack++;
                hasBrokenRibs = true;
                if (!adrenalineActive)
                    healthBar.TakeDamage(20f * brokenRibsStack);
                ShowInjuryMessage("Broken Ribs: -" + (20f * brokenRibsStack) + " HP");
                break;

            case HurtboxType.Legs:
                if (!hasShatteredLegs)
                {
                    TryGetComponent<PlayerMovement>(out var move);
                    move.speed = 4f;
                    hasShatteredLegs = true;
                    ShowInjuryMessage("Shattered Legs: -20% Speed, +10% Hit Chance");
                }
                break;

            case HurtboxType.Head:
                if (!hasHaze)
                {
                    hasHaze = true;
                    ShowInjuryMessage("Haze: +3f Delay, Screen Dark");
                }
                break;
        }

        // Reset damage tracking
        foreach (var key in new List<HurtboxType>(damageMap.Keys))
        {
            damageMap[key] = 0f;
        }
    }

    private void ShowInjuryMessage(string message)
    {
        if (injuryText == null) return;

        // Stop any existing message coroutine
        if (currentMessageCoroutine != null)
        {
            StopCoroutine(currentMessageCoroutine);
        }

        // Set and show the message
        injuryText.text = message;
        injuryText.gameObject.SetActive(true);
        injuryText.alpha = 1f; // Ensure it's fully visible

        // Start new coroutine to hide the message after duration
        currentMessageCoroutine = StartCoroutine(HideMessageAfterDelay());
    }

    private System.Collections.IEnumerator HideMessageAfterDelay()
    {
        yield return new WaitForSeconds(messageDuration);

        // Fade out the text
        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            injuryText.alpha = Mathf.Lerp(1f, 0f, timer / fadeDuration);
            yield return null;
        }

        injuryText.gameObject.SetActive(false);
        currentMessageCoroutine = null;
    }

    public void ActivateAdrenaline()
    {
        adrenalineActive = true;

        if (hasShatteredLegs && TryGetComponent<PlayerMovement>(out var move))
        {
            // optional
        }

        if (hasHaze)
        {
            // Optional: disable visual effects and attack delays here
        }

        if (hasBrokenRibs)
        {
            // Restore temporary HP
            healthBar.Heal(20f * brokenRibsStack);
        }
    }

    public void DeactivateAdrenaline()
    {
        adrenalineActive = false;

        if (hasShatteredLegs && TryGetComponent<PlayerMovement>(out var move))
        {
            move.speed = 4f;
        }

        if (hasHaze)
        {
            // Optional: re-enable haze effects
        }

        if (hasBrokenRibs)
        {
            healthBar.TakeDamage(20f * brokenRibsStack);
        }
    }

    public bool HasShatteredLegs => hasShatteredLegs;
    public bool HasHaze => hasHaze;
    public bool HasBrokenRibs => hasBrokenRibs;
}