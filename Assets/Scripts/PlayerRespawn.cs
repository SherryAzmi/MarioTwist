using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private BoxCollider2D boxCollider;
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
        if (boxCollider == null) boxCollider = GetComponent<BoxCollider2D>();
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
        transform.position = GroundSnapped(checkpointPosition);
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (recoilJump != null)
        {
            recoilJump.ResetGravity();
            recoilJump.RefillBlueGun();
        }
        if (audioSource != null && checkpointClip != null) audioSource.PlayOneShot(checkpointClip, AudioManager.SFXVolume);

        if (protectionRoutine != null) StopCoroutine(protectionRoutine);
        protectionRoutine = StartCoroutine(PlayProtection());
    }

    // Checkpoint markers are hand-placed and don't always sit exactly on the
    // ground surface, so teleporting straight to a stored marker position can
    // leave the player's collider a little embedded in the ground. Normally
    // physics resolves that overlap within a frame or two and it's barely
    // noticeable, but the Kinematic freeze during PlayProtection holds the
    // player at that exact spot for the whole protection window, making any
    // sunken start (then a sudden pop once physics resumes) clearly visible.
    private Vector3 GroundSnapped(Vector3 targetPosition)
    {
        if (boxCollider == null) return targetPosition;

        var hits = Physics2D.RaycastAll(new Vector2(targetPosition.x, targetPosition.y + 5f), Vector2.down, 20f);
        foreach (var hit in hits)
        {
            if (hit.collider == null || hit.collider.isTrigger || hit.collider.gameObject == gameObject) continue;

            float halfHeight = boxCollider.size.y * transform.localScale.y / 2f;
            float colliderOffsetY = boxCollider.offset.y * transform.localScale.y;
            targetPosition.y = hit.point.y + halfHeight - colliderOffsetY;
            return targetPosition;
        }

        return targetPosition;
    }

    // The VFX prefab is centered on its own origin, so instantiating it at
    // transform.position (the player's sprite-center pivot) puts it around the
    // stomach instead of rising up from the ground. Use the collider's bottom
    // edge instead so the effect starts at the feet and builds upward.
    private Vector3 FeetPosition()
    {
        if (boxCollider == null) return transform.position;

        float halfHeight = boxCollider.size.y * transform.localScale.y / 2f;
        float colliderOffsetY = boxCollider.offset.y * transform.localScale.y;
        Vector3 feet = transform.position;
        feet.y = transform.position.y + colliderOffsetY - halfHeight;
        return feet;
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
            vfxInstance = Instantiate(protectionVfxPrefab, FeetPosition(), Quaternion.identity);
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
