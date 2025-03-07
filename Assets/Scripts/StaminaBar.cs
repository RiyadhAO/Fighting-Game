using UnityEngine;
using UnityEngine.UI;

public class ComposureBar : MonoBehaviour
{
    public Slider ComposureSlider;
    public float maxComposure = 50f;
    public float currentComposure;
    public float lerpSpeed = 0.05f;
    public float regenRate = 5f;

    private void Start()
    {
        currentComposure = maxComposure;
        ComposureSlider.maxValue = maxComposure;
        ComposureSlider.value = currentComposure;
    }

    private void Update()
    {
        // Regenerate stamina over time
        if (currentComposure < maxComposure)
        {
            currentComposure += regenRate * Time.deltaTime;
            currentComposure = Mathf.Clamp(currentComposure, 0, maxComposure);
        }

        // Update UI sliders
        ComposureSlider.value = currentComposure;
    }

    public void ReduceComposure(float amount)
    {
        currentComposure -= amount;
        currentComposure = Mathf.Clamp(currentComposure, 0, maxComposure);
    }
}

