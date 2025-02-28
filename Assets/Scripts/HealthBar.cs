using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 150f; // Increased from 100 to 150
    public float health;
    public float lerpSpeed = 0.05f;

    void Start()
    {
        health = maxHealth;
        healthSlider.maxValue = maxHealth;
        easeHealthSlider.maxValue = maxHealth;
        healthSlider.value = health;
        easeHealthSlider.value = health;
    }

    void Update()
    {
        healthSlider.value = health;

        if (healthSlider.value != easeHealthSlider.value)
        {
            easeHealthSlider.value = Mathf.Lerp(easeHealthSlider.value, health, lerpSpeed);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        health = Mathf.Clamp(health, 0, maxHealth); // Ensure health doesn't go below 0 or above maxHealth
    }

    public void Heal(float amount)
    {
        health += amount;
        health = Mathf.Clamp(health, 0, maxHealth);
    }
}

