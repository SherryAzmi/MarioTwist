using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private AudioClip gameOverClip;
    [SerializeField] private CameraShake cameraShake;

    public int MaxHealth => maxHealth;
    public int CurrentHealth { get; private set; }
    public bool IsGameOver { get; private set; }

    public event System.Action<int, int> HealthChanged;
    public event System.Action GameOver;

    private void Awake()
    {
        CurrentHealth = maxHealth;
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (cameraShake == null) cameraShake = GetComponentInChildren<CameraShake>();
    }

    public void Kill()
    {
        TakeDamage(CurrentHealth);
    }

    public void TakeDamage(int amount)
    {
        if (IsGameOver) return;

        CurrentHealth = Mathf.Max(0, CurrentHealth - amount);
        HealthChanged?.Invoke(CurrentHealth, maxHealth);

        if (damageClip != null) audioSource.PlayOneShot(damageClip);
        if (cameraShake != null) cameraShake.Shake();

        if (CurrentHealth <= 0)
        {
            IsGameOver = true;
            if (gameOverClip != null) audioSource.PlayOneShot(gameOverClip);
            GameOver?.Invoke();

            var recoilJump = GetComponent<PlayerRecoilJump>();
            if (recoilJump != null) recoilJump.enabled = false;
        }
    }
}
