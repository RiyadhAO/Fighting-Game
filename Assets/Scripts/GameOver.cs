using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class GameOver : MonoBehaviour
{
    public GameObject gameOverUI;

    void Start()
    {
        // Optional, depends on your specific UI needs
        // Canvas.ForceUpdateCanvases(); 
    }

    private void Awake()
    {
        // Force Unity to process UI even if timeScale is 0
        EventSystem.current.sendNavigationEvents = true;
    }

    public void RestartGame()
    {
        Debug.Log("Restart Button Pressed");

        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        GameAnalyticsManager.Instance.TrackGameModeStart("MainGame");
    }

    public void GoToMainMenu()
    {
        Debug.Log("Main Menu Button Pressed");

        Time.timeScale = 1f; // Resume time
        SceneManager.LoadScene("Demo1"); // Change to correct scene name
    }

    void Update()
    {
        if (gameOverUI.activeSelf) // Corrected check for active UI
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                RestartGame();
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                GoToMainMenu();
            }
        }
    }
}

