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
    [SerializeField] private Color slowGunColor = Color.red;
    [SerializeField] private Color fastGunColor = Color.blue;
    [SerializeField] private float slowGunSpeedMultiplier = 0.5f;
    [SerializeField] private float slowGunGravityMultiplier = 0.4f;
    [SerializeField] private float redFireRate = 0.2f;
    [SerializeField] private float fallDamageSpeedThreshold = 6f;
    [SerializeField] private int fallDamage = 5;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip fastJumpClip;

    private float defaultGravityScale;
    private bool slowFalling;
    private bool isGrounded;
    private float lastRedFireTime = -999f;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (weaponAim == null) weaponAim = GetComponentInChildren<WeaponAim>();
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
            PerformJump(jumpForce * slowGunSpeedMultiplier, slowGunColor, jumpDirection, jumpClip, slowGravity, isSlow: true);
        }
        else
        {
            PerformJump(fastJumpForce, fastGunColor, jumpDirection, fastJumpClip, defaultGravityScale, isSlow: false);
        }
    }

    private void PerformJump(float force, Color gunColor, Vector2 jumpDirection, AudioClip clip, float gravityScale, bool isSlow)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip);
        if (weaponAim.SpriteRenderer != null) weaponAim.SpriteRenderer.color = gunColor;

        rb.linearVelocity = jumpDirection * force;
        rb.gravityScale = gravityScale;
        slowFalling = isSlow;

        isGrounded = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;

        float fallSpeed = Mathf.Abs(collision.relativeVelocity.y);
        if (!slowFalling && fallSpeed > fallDamageSpeedThreshold && playerHealth != null)
        {
            playerHealth.TakeDamage(fallDamage);
        }

        if (slowFalling)
        {
            rb.gravityScale = defaultGravityScale;
            slowFalling = false;
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
