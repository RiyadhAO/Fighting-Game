using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StopwatchCountdown : MonoBehaviour
{
    [Header("UI References")]
    public TMP_Text timerText;
    public Image redScreenEffect;

    [Header("Timer Settings")]
    public float startTime = 100f;
    [Tooltip("Speed at which the timer counts down (1 = normal speed)")]
    public float lerpSpeed = 1f;
    [SerializeField] private float _currentTime;
    private bool _isRunning = true;
    private Coroutine _countdownCoroutine;
    private Coroutine _overtimeCoroutine;

    [Header("Overtime Settings")]
    public float healthPenalty = 10f;
    public HealthBar player1Health;
    public HealthBar player2Health;
    public Color overtimeColor = new Color(0.5f, 0f, 0f, 0.2f);
    public string overtimeText = "Overtime";

    public float CurrentTime
    {
        get => _currentTime;
        private set
        {
            _currentTime = Mathf.Max(0, value);
            UpdateTimerUI();
        }
    }

    public bool IsInOvertime { get; private set; }

    private void Start()
    {
        ResetTimer();
        _countdownCoroutine = StartCoroutine(Countdown());
        PauseTimer();
    }

    private void ResetTimer()
    {
        CurrentTime = startTime;
        IsInOvertime = false;
        UpdateTimerUI();

        // Reset red screen effect
        if (redScreenEffect != null)
        {
            redScreenEffect.color = new Color(overtimeColor.r, overtimeColor.g, overtimeColor.b, 0f);
        }
    }

    private System.Collections.IEnumerator Countdown()
    {
        while (CurrentTime > 0 && _isRunning)
        {
            float elapsedTime = 0f;
            float previousTime = CurrentTime;
            float targetTime = Mathf.Max(0, CurrentTime - 1);

            while (elapsedTime < 1f && _isRunning)
            {
                elapsedTime += Time.deltaTime * lerpSpeed;
                CurrentTime = Mathf.Lerp(previousTime, targetTime, elapsedTime);
                yield return null;
            }

            if (_isRunning) CurrentTime = targetTime;
        }

        if (_isRunning)
        {
            _overtimeCoroutine = StartCoroutine(Overtime());
        }
    }

    private System.Collections.IEnumerator Overtime()
    {
        IsInOvertime = true;
        timerText.text = overtimeText;

        // Pulsing setup
        float pulseSpeed = 2f;
        float minAlpha = 0.01f;
        float maxAlpha = overtimeColor.a;
        float elapsedTime = 0f;

        // Damage timing
        float damageInterval = 1f; // Apply damage every 1 second
        float nextDamageTime = Time.time + damageInterval;

        while (_isRunning && CurrentTime <= 0)
        {
            // --- Handle visual pulsing ---
            float alpha = Mathf.Lerp(minAlpha, maxAlpha,
                (Mathf.Sin(elapsedTime * pulseSpeed) * 0.5f + 0.5f));

            redScreenEffect.color = new Color(
                overtimeColor.r,
                overtimeColor.g,
                overtimeColor.b,
                alpha
            );
            elapsedTime += Time.deltaTime;

            // --- Handle damage (once per second) ---
            if (Time.time >= nextDamageTime)
            {
                TriggerTimeOutEffect();
                nextDamageTime = Time.time + damageInterval;
            }

            yield return null;
        }

        // Cleanup
        if (CurrentTime > 0)
        {
            IsInOvertime = false;
            redScreenEffect.color = new Color(overtimeColor.r, overtimeColor.g, overtimeColor.b, 0f);
            _countdownCoroutine = StartCoroutine(Countdown());
        }
    }

    private void UpdateTimerUI()
    {
        timerText.text = CurrentTime > 0 ? Mathf.Ceil(CurrentTime).ToString() : overtimeText;
    }

    private void TriggerTimeOutEffect()
    {
        if (player1Health != null) player1Health.TakeDamage(healthPenalty);
        if (player2Health != null) player2Health.TakeDamage(healthPenalty);
    }

    public void PauseTimer()
    {
        _isRunning = false;
    }

    public void ResumeTimer()
    {
        if (_isRunning) return;

        _isRunning = true;
        _countdownCoroutine = StartCoroutine(Countdown());
    }

    public void AddTime(float seconds)
    {
        CurrentTime += seconds;

        // If we were in overtime and now have time again
        if (IsInOvertime && CurrentTime > 0)
        {
            IsInOvertime = false;
            if (_overtimeCoroutine != null)
            {
                StopCoroutine(_overtimeCoroutine);
            }

            // Clear the red screen
            if (redScreenEffect != null)
            {
                redScreenEffect.color = new Color(overtimeColor.r, overtimeColor.g, overtimeColor.b, 0f);
            }

            // Restart normal countdown
            _countdownCoroutine = StartCoroutine(Countdown());
        }
    }

    public void SetTimer(float newTime, bool resetImmediately = false)
    {
        startTime = newTime;
        if (resetImmediately)
        {
            ResetTimer();
            if (_countdownCoroutine != null)
            {
                StopCoroutine(_countdownCoroutine);
                _countdownCoroutine = StartCoroutine(Countdown());
            }
        }
    }
}