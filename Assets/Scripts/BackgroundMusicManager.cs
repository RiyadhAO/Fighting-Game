using UnityEngine;

public class BackgroundMusicManager : MonoBehaviour
{
    public AudioClip[] musicTracks; // Assign songs in the Inspector
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        PlayAndLoopRandomSong();
    }

    void PlayAndLoopRandomSong()
    {
        if (musicTracks.Length == 0) return;

        int randomIndex = Random.Range(0, musicTracks.Length);
        audioSource.clip = musicTracks[randomIndex];
        audioSource.loop = true; // Loop the selected track
        audioSource.Play();
    }
}
