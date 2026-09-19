using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxLives = 3;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private PlayerRespawn playerRespawn;

    public int MaxLives => maxLives;
    public int CurrentLives { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsInvulnerable { get; private set; }

    public event System.Action<int, int> LivesChanged;
    public event System.Action GameOver;

    private void Awake()
    {
        CurrentLives = maxLives;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (playerRespawn == null) playerRespawn = GetComponent<PlayerRespawn>();
    }

    public void SetInvulnerable(bool invulnerable)
    {
        IsInvulnerable = invulnerable;
    }

    public void LoseHeart()
    {
        if (IsGameOver || IsInvulnerable) return;

        CurrentLives = Mathf.Max(0, CurrentLives - 1);
        LivesChanged?.Invoke(CurrentLives, maxLives);

        if (damageClip != null) audioSource.PlayOneShot(damageClip, AudioManager.SFXVolume);

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
