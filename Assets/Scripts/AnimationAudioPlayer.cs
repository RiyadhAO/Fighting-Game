using UnityEngine;

public class AnimationAudioPlayer : MonoBehaviour
{
    public AudioSource audioSource; // Assign in the Inspector
    public AudioClip[] audioClips; // Assign different sounds in the Inspector

    private void Start()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                Debug.LogError("No AudioSource found! Please assign one in the Inspector.");
            }
        }
    }

    // This function can be called from Animation Events
    public void PlaySound(int clipIndex)
    {
        if (audioSource == null || audioClips == null || audioClips.Length == 0)
        {
            Debug.LogWarning("Missing AudioSource or audio clips!");
            return;
        }

        if (clipIndex >= 0 && clipIndex < audioClips.Length)
        {
            audioSource.PlayOneShot(audioClips[clipIndex]);
        }
        else
        {
            Debug.LogWarning("Invalid clip index!");
        }
    }
}
