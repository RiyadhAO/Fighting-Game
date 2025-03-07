using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class KnockdownTracker : MonoBehaviour
{
    public int maxKnockdowns = 3;
    private int knockdownCount = 0;

    [Header("UI Elements")]
    public GameObject gameOverUI;
    public TMP_Text gameOverText;
    public Button restartButton;
    public Button mainMenuButton;
    public Slider KnockdownSlider;

    public string playerName;

    void Start()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
        KnockdownSlider.maxValue = maxKnockdowns;
        KnockdownSlider.value = knockdownCount;
    }

    public void RegisterKnockdown()
    {
        knockdownCount++;
        KnockdownSlider.value = knockdownCount;

        if (knockdownCount >= maxKnockdowns)
        {
            EndGame();
        }
    }

    private void EndGame()
    {
        Debug.Log(playerName + " has been knocked down 3 times! Game Over!");

        // Show the end game UI
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
