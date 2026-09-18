using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int maxLives = 3;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private CameraShake cameraShake;
    [SerializeField] private PlayerRespawn playerRespawn;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public int MaxLives => maxLives;
    public int CurrentLives { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsInvulnerable { get; private set; }

    public event System.Action<int, int> HealthChanged;
    public event System.Action<int, int> LivesChanged;
    public event System.Action GameOver;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        CurrentLives = maxLives;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (cameraShake == null) cameraShake = GetComponentInChildren<CameraShake>();
        if (playerRespawn == null) playerRespawn = GetComponent<PlayerRespawn>();
    }

    public void Kill()
    {
        TakeDamage(CurrentHealth);
    }

    public void SetInvulnerable(bool invulnerable)
    {
        IsInvulnerable = invulnerable;
    }

    public void TakeDamage(int amount)
    {
        if (IsGameOver || IsInvulnerable) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        HealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (damageClip != null) audioSource.PlayOneShot(damageClip, AudioManager.SFXVolume);
        if (cameraShake != null) cameraShake.Shake();

        if (CurrentHealth <= 0) TriggerGameOver();
    }

    public void LoseHeart()
    {
        if (IsGameOver || IsInvulnerable) return;

        CurrentLives = Mathf.Max(0, CurrentLives - 1);
        LivesChanged?.Invoke(CurrentLives, maxLives);

        if (damageClip != null) audioSource.PlayOneShot(damageClip, AudioManager.SFXVolume);
        if (cameraShake != null) cameraShake.Shake();

        if (CurrentLives <= 0)
        {
            TriggerGameOver();
        }
        else if (playerRespawn != null)
        {
            playerRespawn.RespawnAtCheckpoint();
        }
    }

    private void TriggerGameOver()
    {
        IsGameOver = true;

        // Silence everything else in the game (hazard loops, any background music
        // that gets added later) so only the game-over clip is audible.
        var allAudioSources = FindObjectsByType<AudioSource>(FindObjectsSortMode.None);
        foreach (var src in allAudioSources)
        {
            if (src != audioSource) src.Stop();
        }

        if (gameOverClip != null) audioSource.PlayOneShot(gameOverClip, AudioManager.SFXVolume);
        GameOver?.Invoke();

        var recoilJump = GetComponent<PlayerRecoilJump>();
        if (recoilJump != null) recoilJump.enabled = false;
    }
}
