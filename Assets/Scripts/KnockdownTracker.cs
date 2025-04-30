using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KnockdownTracker : MonoBehaviour
{
    public int maxKnockdowns = 3;
    public int knockdownCount = 0;

    [Header("UI Elements")]
    public GameObject gameOverUI;
    public TMP_Text gameOverText;
    public Button restartButton;
    public Button mainMenuButton;

    public Image[] knockdownIcons; // Assign in Inspector
    public Sprite activeKnockdownSprite;  // Default icon
    public Sprite inactiveKnockdownSprite; // Icon for lost knockdowns

    public string playerName;

    void Start()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);

        UpdateKnockdownUI();
    }

    public void RegisterKnockdown()
    {
        if (knockdownCount < maxKnockdowns)
        {
            knockdownCount++;
            UpdateKnockdownUI();
        }

        if (knockdownCount >= maxKnockdowns)
        {
            EndGame();
        }
    }

    private void UpdateKnockdownUI()
    {
        for (int i = 0; i < knockdownIcons.Length; i++)
        {
            if (i < knockdownCount)
            {
                knockdownIcons[i].sprite = inactiveKnockdownSprite; // Gray out or remove
            }
            else
            {
                knockdownIcons[i].sprite = activeKnockdownSprite; // Show remaining knockdowns
            }
        }
    }

    private void EndGame()
    {
        Debug.Log(playerName + " has been knocked down 3 times! Game Over!");
        GameAnalyticsManager.Instance.TrackMatchResult(playerName, false);

        if (gameOverUI != null)
        {
            gameOverUI.SetActive(true);
            if (gameOverText != null)
            {
                gameOverText.text = playerName + " is KO!";
                Time.timeScale = 0f;
            }
        }

        restartButton.interactable = true;
        mainMenuButton.interactable = true;

        Time.timeScale = 0f;
    }
}
