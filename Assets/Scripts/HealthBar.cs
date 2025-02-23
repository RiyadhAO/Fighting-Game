using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Slider healthSlider;
    public Slider easeHealthSlider;
    public float maxHealth = 100f;
    public float health;
    public float lerpSpeed = 0.05f;

    void Start()
    {
        health = maxHealth;
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
    }
}
