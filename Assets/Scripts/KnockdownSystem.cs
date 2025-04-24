using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class KnockdownSystem : MonoBehaviour
{
    public CharacterInjury injurySystem;
    public Animator animator;
    public float knockdownTime = 3f;
    public bool isKnockedDown = false;
    public float bonusTime = 30f;

    public HealthBar healthBar; // Assigned in Unity Editor
    public KnockdownTracker knockdownTracker;

    private PlayerInput playerInput;
    public PlayerInput EnemyPlayerInput;
    private Rigidbody rb;
    public StopwatchCountdown stopwatch;

    public AdrenalineBar adrenaline;

    void Start()
    {
        playerInput = GetComponent<PlayerInput>();
        rb = GetComponent<Rigidbody>();

        if (knockdownTracker == null)
            Debug.LogError("KnockdownTracker not assigned on " + gameObject.name);

        if (healthBar == null)
            Debug.LogError("HealthBar not assigned in the editor!");

        if (injurySystem == null)
            injurySystem = GetComponent<CharacterInjury>();
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
        stopwatch.PauseTimer();

        // Disable player input during knockdown
        if (playerInput != null)
        {
            EnemyPlayerInput.enabled = false;
            playerInput.enabled = false;
        }

        // Stop movement completely & freeze Rigidbody
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;  // Freezes physics movement
        }

        // Register knockdown before waiting
        if (knockdownTracker != null)
        {
            knockdownTracker.RegisterKnockdown();
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
        adrenaline.ActivateAdrenaline();

        CharacterInjury injury = GetComponent<CharacterInjury>();
        if (injury != null)
        {
            injury.EvaluateInjuryAfterKnockdown();
        }

        // Re-enable input
        if (playerInput != null)
        {
            EnemyPlayerInput.enabled = true;
            playerInput.enabled = true;
        }

        // Unfreeze Rigidbody so the player can move again
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (stopwatch != null && stopwatch.IsInOvertime)
        {
            stopwatch.AddTime(bonusTime);
        }
        stopwatch.ResumeTimer();
        isKnockedDown = false;
    }
}
