using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PracticeManager : MonoBehaviour
{
    public GameObject character;
    public GameObject character2;

    public HealthBar healthBar;
    public HealthBar healthBar2;

    public AdrenalineBar adbar;
    public AdrenalineBar adbar2;

    private void Start()
    {
        adbar.ActivateAdrenaline();
        adbar2.ActivateAdrenaline();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene("Demo1");
        }

        if (healthBar.health < 80)
        {
            healthBar.health = healthBar.maxHealth;
        }

        if (healthBar2.health < 80)
        {
            healthBar2.health = healthBar2.maxHealth;
        }
    }
}