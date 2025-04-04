using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks; // Assign songs in the Inspector
    private AudioSource audioSource;
    private int lastPlayedIndex = -1; // Store last played song index

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayRandomSong();
    }

    void Update()
    {
        // Check if music has stopped playing
        if (!audioSource.isPlaying)
        {
            PlayRandomSong();
        }
    }

    void PlayRandomSong()
    {
        if (musicTracks.Length == 0) return; // No tracks assigned

        int randomIndex;
        do
        {
            randomIndex = Random.Range(0, musicTracks.Length);
        } while (randomIndex == lastPlayedIndex); // Ensure different song plays

        lastPlayedIndex = randomIndex; // Save last played song
        audioSource.clip = musicTracks[randomIndex];
        audioSource.loop = false; // Ensure it doesn't loop
        audioSource.Play();
    }
}

