using System.Collections;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public HealthBar healthBar;

    public void TakeDamage(float damage)
    {
        healthBar.health -= damage;
    }
}

