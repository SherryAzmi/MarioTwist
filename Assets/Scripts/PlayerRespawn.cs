using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerRecoilJump recoilJump;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip checkpointClip;
    [SerializeField] private GameObject protectionVfxPrefab;
    [SerializeField] private float protectionDuration = 1.5f;

    private Vector3 checkpointPosition;
    private Coroutine protectionRoutine;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (recoilJump == null) recoilJump = GetComponent<PlayerRecoilJump>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        checkpointPosition = transform.position;
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
    }

    public void RespawnAtCheckpoint()
    {
        transform.position = checkpointPosition;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (recoilJump != null) recoilJump.ResetGravity();
        if (audioSource != null && checkpointClip != null) audioSource.PlayOneShot(checkpointClip, AudioManager.SFXVolume);

        if (protectionRoutine != null) StopCoroutine(protectionRoutine);
        protectionRoutine = StartCoroutine(PlayProtection());
    }

    private System.Collections.IEnumerator PlayProtection()
    {
        if (playerHealth != null) playerHealth.SetInvulnerable(true);
        if (recoilJump != null) recoilJump.enabled = false;

        RigidbodyType2D originalBodyType = RigidbodyType2D.Dynamic;
        if (rb != null)
        {
            originalBodyType = rb.bodyType;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        GameObject vfxInstance = null;
        if (protectionVfxPrefab != null)
        {
            vfxInstance = Instantiate(protectionVfxPrefab, transform.position, Quaternion.identity);
        }

        yield return new WaitForSeconds(protectionDuration);

        if (rb != null) rb.bodyType = originalBodyType;
        if (recoilJump != null) recoilJump.enabled = true;
        if (playerHealth != null) playerHealth.SetInvulnerable(false);

        if (vfxInstance != null)
        {
            // Stop spawning new particles now (matches the gameplay-facing protection
            // window), but let already-alive particles fade out naturally afterward
            // instead of vanishing instantly -- the player is free to move by then.
            var rootParticles = vfxInstance.GetComponent<ParticleSystem>();
            if (rootParticles != null) rootParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            Destroy(vfxInstance, 5f);
        }

        protectionRoutine = null;
    }
}
