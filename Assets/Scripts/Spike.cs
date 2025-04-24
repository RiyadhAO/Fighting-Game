using UnityEngine;

public class Spike : MonoBehaviour
{
    public int damage = 50;
    public AudioClip damageSound; // Assign in Inspector
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            // Automatically add AudioSource if not present
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something entered the spike trigger: " + other.name);

        PlayerState state = other.GetComponentInParent<PlayerState>();
        KnockdownSystem sys = other.GetComponentInParent<KnockdownSystem>();

        if (state != null && state.wasPushed && !state.hasTakenSpikeDamage && sys != null)
        {
            sys.healthBar.TakeDamage(damage);
            state.hasTakenSpikeDamage = true; // Prevent double-hit
            Debug.Log("Player damaged by spike!");

            if (damageSound != null)
            {
                audioSource.PlayOneShot(damageSound);
            }
        }
    }
}
