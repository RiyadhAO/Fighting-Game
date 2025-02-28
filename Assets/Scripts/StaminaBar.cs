using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaminaBar : MonoBehaviour
{
    public float maxStamina = 50f;
    public float currentStamina;
    public float regenRate = 5f;

    private void Start()
    {
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (currentStamina < maxStamina)
        {
            currentStamina += regenRate * Time.deltaTime;
        }
    }

    public void ReduceStamina(float amount)
    {
        currentStamina -= amount;
        currentStamina = Mathf.Max(currentStamina, 0);
    }
}

