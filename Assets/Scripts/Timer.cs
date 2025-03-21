using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StopwatchCountdown : MonoBehaviour
{
    public TMP_Text timerText; // Reference to the TMP Text component
    public float startTime = 100f; // Starting time
    private float currentTime;
    public float lerpSpeed = 1f;
    private bool isRunning = true;

    public Image redScreenEffect; // Reference to the UI Image for red flash effect
    public float flashDuration = 0.5f; // Duration of the red flash
    public HealthBar playerHealth; // Reference to the player's health bar
    public HealthBar player2Health;
    public float healthPenalty = 10f; // Amount of health to reduce every second after time runs out

    private void Start()
    {
        currentTime = startTime;
        UpdateTimerUI();
        StartCoroutine(Countdown());
    }

    private System.Collections.IEnumerator Countdown()
    {
        while (currentTime > 0 && isRunning)
        {
            float elapsedTime = 0f;
            float previousTime = currentTime;
            float targetTime = Mathf.Max(0, currentTime - 1);

            while (elapsedTime < 1f)
            {
                elapsedTime += Time.deltaTime * lerpSpeed;
                currentTime = Mathf.Lerp(previousTime, targetTime, elapsedTime);
                UpdateTimerUI();
                yield return null;
            }

            currentTime = targetTime;
            UpdateTimerUI();
        }

        StartCoroutine(Overtime());
    }

    private System.Collections.IEnumerator Overtime()
    {
        while (true)
        {
            timerText.text = "Overtime";
            TriggerTimeOutEffect();
            yield return new WaitForSeconds(1f);
        }
    }

    private void UpdateTimerUI()
    {
        if (currentTime > 0)
        {
            timerText.text = Mathf.Ceil(currentTime).ToString(); // Display rounded time
        }
        else
        {
            timerText.text = "Overtime";
        }
    }

    private void TriggerTimeOutEffect()
    {
        if (playerHealth != null)
        {
            playerHealth.TakeDamage(healthPenalty);
            player2Health.TakeDamage(healthPenalty);
        }
        if (redScreenEffect != null)
        {
            StartCoroutine(FlashRedScreen());
        }
    }

    private System.Collections.IEnumerator FlashRedScreen()
    {
        redScreenEffect.color = new Color(1f, 0f, 0f, 0.3f); // Semi-transparent red
        yield return new WaitForSeconds(flashDuration);
        redScreenEffect.color = new Color(1f, 0f, 0f, 0f); // Fully transparent
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}