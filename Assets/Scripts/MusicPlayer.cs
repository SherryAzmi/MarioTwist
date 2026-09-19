using UnityEngine;

// Plays this scene's background music. Each scene that wants music has its own MusicPlayer with its own clip.
// It is not kept between scenes, so leaving a scene stops its track and the next scene starts its own.
public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip musicClip;

    private void Awake()
    {
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = AudioManager.MusicVolume;
    }

    private void OnEnable()
    {
        AudioManager.MusicVolumeChanged += SetVolume;
    }

    private void OnDisable()
    {
        AudioManager.MusicVolumeChanged -= SetVolume;
    }

    private void Start()
    {
        audioSource.Play();
    }

    private void SetVolume(float volume)
    {
        if (audioSource != null) audioSource.volume = volume;
    }
}
