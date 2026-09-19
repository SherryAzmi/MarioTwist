using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerRecoilJump recoilJump;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip checkpointClip;
    [SerializeField] private GameObject protectionVfxPrefab;
    [SerializeField] private float protectionVfxScale = 0.5f;
    [SerializeField] private float protectionDuration = 1.5f;
    [SerializeField] private float protectionFadeOutDuration = 0.5f;

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
            vfxInstance.transform.localScale = Vector3.one * protectionVfxScale;
        }

        yield return new WaitForSeconds(protectionDuration);

        // Unlock movement/damage and start the VFX's shrink-out in the same instant,
        // so the fade is the visible cue that protection just ended -- the particle
        // system's own lifetime is too slow/subtle on its own to read as "fading".
        if (rb != null) rb.bodyType = originalBodyType;
        if (recoilJump != null) recoilJump.enabled = true;
        if (playerHealth != null) playerHealth.SetInvulnerable(false);

        if (vfxInstance != null) StartCoroutine(FadeOutVfx(vfxInstance));

        protectionRoutine = null;
    }

    private System.Collections.IEnumerator FadeOutVfx(GameObject vfxInstance)
    {
        var rootParticles = vfxInstance.GetComponent<ParticleSystem>();
        if (rootParticles != null) rootParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        Vector3 startScale = vfxInstance.transform.localScale;
        float elapsed = 0f;
        while (elapsed < protectionFadeOutDuration)
        {
            elapsed += Time.deltaTime;
            vfxInstance.transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsed / protectionFadeOutDuration);
            yield return null;
        }

        Destroy(vfxInstance);
    }
}
