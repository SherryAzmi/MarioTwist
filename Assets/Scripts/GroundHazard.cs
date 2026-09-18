using UnityEngine;

public class GroundHazard : MonoBehaviour
{
    [SerializeField] private PlayerRecoilJump recoilJump;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float damageInterval = 0.5f;
    [SerializeField] private int damagePerTick = 2;
    [SerializeField] private AudioSource hazardAudioSource;
    [SerializeField] private AudioClip hazardLoopClip;

    private float tickTimer;

    private void Awake()
    {
        if (recoilJump == null) recoilJump = GetComponent<PlayerRecoilJump>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();

        if (hazardAudioSource != null)
        {
            hazardAudioSource.clip = hazardLoopClip;
            hazardAudioSource.loop = true;
            hazardAudioSource.playOnAwake = false;
        }
    }

    private void Update()
    {
        if (recoilJump == null || playerHealth == null || !recoilJump.IsGrounded)
        {
            tickTimer = 0f;
            if (hazardAudioSource != null && hazardAudioSource.isPlaying) hazardAudioSource.Stop();
            return;
        }

        if (hazardAudioSource != null && hazardLoopClip != null && !hazardAudioSource.isPlaying)
        {
            hazardAudioSource.Play();
        }

        tickTimer += Time.deltaTime;
        if (tickTimer >= damageInterval)
        {
            tickTimer -= damageInterval;
            playerHealth.TakeDamage(damagePerTick);
        }
    }
}
