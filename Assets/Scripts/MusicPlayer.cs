using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip musicClip;

    private static MusicPlayer instance;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);

        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        audioSource.clip = musicClip;
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = AudioManager.MusicVolume;
    }

    private void OnEnable()
    {
        AudioManager.MusicVolumeChanged += SetVolume;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        AudioManager.MusicVolumeChanged -= SetVolume;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        audioSource.Play();
    }

    // Covers TriggerGameOver's blanket AudioSource.Stop() -- resume automatically
    // whenever a new scene loads (restart, back to main menu) instead of staying silent.
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (!audioSource.isPlaying) audioSource.Play();
    }

    private void SetVolume(float volume)
    {
        if (audioSource != null) audioSource.volume = volume;
    }
}
