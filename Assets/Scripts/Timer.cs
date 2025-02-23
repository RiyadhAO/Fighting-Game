using UnityEngine;
using TMPro;

public class StopwatchCountdown : MonoBehaviour
{
    public TMP_Text timerText; // Reference to the TMP Text component
    public float startTime = 100f; // Starting time
    private float currentTime;
    public float lerpSpeed = 1f;
    private bool isRunning = true;

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
    }

    private void UpdateTimerUI()
    {
        timerText.text = Mathf.Ceil(currentTime).ToString(); // Display rounded time
    }

    public void StopTimer()
    {
        isRunning = false;
    }
}
