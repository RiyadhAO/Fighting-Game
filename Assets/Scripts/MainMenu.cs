using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public CanvasGroup OptionPanel;
    public CanvasGroup MainPanel;

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void Option()
    {
        OptionPanel.alpha = 1;
        OptionPanel.blocksRaycasts = true;
        MainPanel.blocksRaycasts = false;
    }

    public void Back()
    {
        OptionPanel.alpha = 0;
        OptionPanel.blocksRaycasts = false;
        MainPanel.blocksRaycasts = true;
    }

    public void Quit()
    {
        Application.Quit();
        Debug.Log("You would have Quit");
    }

    public void Tutorial()
    {
        SceneManager.LoadScene("Tutorial");
        GameAnalyticsManager.Instance.TrackGameModeStart("Tutorial");
    }

    public void Practice()
    {
        SceneManager.LoadScene("Practice");
        GameAnalyticsManager.Instance.TrackGameModeStart("Practice");
    }
}
