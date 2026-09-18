using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRecoilJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private WeaponAim weaponAim;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float fastJumpForce = 25f;
    [SerializeField] private Sprite slowGunSprite;
    [SerializeField] private Sprite fastGunSprite;
    [SerializeField] private ParticleSystem gunParticles;
    [SerializeField] private Color slowGunParticleColor = Color.red;
    [SerializeField] private Color fastGunParticleColor = Color.blue;
    [SerializeField] private float slowGunSpeedMultiplier = 0.5f;
    [SerializeField] private float slowGunGravityMultiplier = 0.4f;
    [SerializeField] private float redFireRate = 0.2f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip fastJumpClip;

    private float defaultGravityScale;
    private bool isGrounded;
    private float lastRedFireTime = -999f;

    public bool IsGrounded => isGrounded;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (weaponAim == null) weaponAim = GetComponentInChildren<WeaponAim>();
        if (gunParticles == null && weaponAim != null) gunParticles = weaponAim.GetComponentInChildren<ParticleSystem>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        defaultGravityScale = rb.gravityScale;
    }

    private void Update()
    {
        if (Mouse.current == null || weaponAim == null) return;

        bool leftHeld = Mouse.current.leftButton.isPressed;
        bool rightClicked = Mouse.current.rightButton.wasPressedThisFrame;
        bool leftReady = leftHeld && Time.time - lastRedFireTime >= redFireRate;

        if (!leftReady && !rightClicked) return;

        Vector2 jumpDirection = -weaponAim.AimDirection;
        bool aimingUp = jumpDirection.y < -0.1f;

        // Aiming up launches downward. If already grounded, that would just push
        // into the floor with no actual movement, so skip the jump entirely.
        if (aimingUp && isGrounded) return;

        if (leftReady)
        {
            lastRedFireTime = Time.time;
            float slowGravity = defaultGravityScale * slowGunGravityMultiplier;
            PerformJump(jumpForce * slowGunSpeedMultiplier, slowGunSprite, slowGunParticleColor, jumpDirection, jumpClip, slowGravity);
        }
        else
        {
            PerformJump(fastJumpForce, fastGunSprite, fastGunParticleColor, jumpDirection, fastJumpClip, defaultGravityScale);
        }
    }

    private void PerformJump(float force, Sprite gunSprite, Color particleColor, Vector2 jumpDirection, AudioClip clip, float gravityScale)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
        if (weaponAim.SpriteRenderer != null && gunSprite != null) weaponAim.SpriteRenderer.sprite = gunSprite;

        if (gunParticles != null)
        {
            var main = gunParticles.main;
            main.startColor = particleColor;
            gunParticles.Play();
        }

        rb.linearVelocity = jumpDirection * force;
        rb.gravityScale = gravityScale;

        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;
        rb.gravityScale = defaultGravityScale;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
