using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerRecoilJump : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private WeaponAim weaponAim;
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float downwardJumpSpeedMultiplier = 0.5f;
    [SerializeField] private float downwardFallGravityMultiplier = 0.4f;
    [SerializeField] private float fallDamageSpeedThreshold = 6f;
    [SerializeField] private int fallDamage = 5;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpClip;

    private float defaultGravityScale;
    private bool slowFalling;

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

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (audioSource != null && jumpClip != null) audioSource.PlayOneShot(jumpClip);

            Vector2 jumpDirection = -weaponAim.AimDirection;
            bool aimingUp = jumpDirection.y < -0.1f;

            if (aimingUp)
            {
                rb.linearVelocity = jumpDirection * jumpForce * downwardJumpSpeedMultiplier;
                rb.gravityScale = defaultGravityScale * downwardFallGravityMultiplier;
                slowFalling = true;
            }
            else
            {
                rb.linearVelocity = jumpDirection * jumpForce;
                rb.gravityScale = defaultGravityScale;
                slowFalling = false;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
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
}
