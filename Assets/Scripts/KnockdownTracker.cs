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

    public string playerName;

    void Start()
    {
        if (gameOverUI != null)
            gameOverUI.SetActive(false);
    }

    public void RegisterKnockdown()
    {
        knockdownCount++;

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
                gameOverText.text = playerName + " Wins!";
            }
        }

        restartButton.interactable = true;
        mainMenuButton.interactable = true;

        // Pause the game
        Time.timeScale = 0f;
    }
}
