using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class KnockdownSystem : MonoBehaviour
{
    public Animator animator;
    public float knockdownTime = 3f;
    public bool isKnockedDown = false;

    public HealthBar healthBar; // Assigned in Unity Editor
    public KnockdownTracker knockdownTracker;

    private PlayerInput playerInput;
    public PlayerInput EnemyPlayerInput;
    private Rigidbody rb;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        if (knockdownTracker == null)
            Debug.LogError("KnockdownTracker not assigned on " + gameObject.name);

        if (healthBar == null)
            Debug.LogError("HealthBar not assigned in the editor!");
    }

    void Update()
    {
        // Knockdown when health reaches zero
        if (!isKnockedDown && healthBar.health <= 0)
        {
            StartCoroutine(HandleKnockdown());
        }
    }

    private IEnumerator HandleKnockdown()
    {
        isKnockedDown = true;
        animator.SetTrigger("Knockdown");

        // Disable player input during knockdown
        if (playerInput != null)
            EnemyPlayerInput.enabled = false;
            playerInput.enabled = false;

        // Stop movement completely
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        yield return new WaitForSeconds(knockdownTime);

        Recover();
    }

    private void Recover()
    {
        animator.SetTrigger("Recover");

        // Reset health and update UI
        healthBar.health = healthBar.maxHealth;
        healthBar.healthSlider.value = healthBar.health;
        healthBar.easeHealthSlider.value = healthBar.health;

        // Register knockdown
        if (knockdownTracker != null)
        {
            knockdownTracker.RegisterKnockdown();
        }

        // Re-enable input
        if (playerInput != null)
        {
            EnemyPlayerInput.enabled = true;
            playerInput.enabled = true;
        }

        isKnockedDown = false;
    }
}
