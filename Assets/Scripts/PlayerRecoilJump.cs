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
    [SerializeField] private GameObject muzzleEffectPrefab;
    [SerializeField] private Color slowGunParticleColor = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color fastGunParticleColor = new Color(1f, 0.9f, 0.1f);
    [SerializeField] private Vector2 slowGunMuzzleOffset = new Vector2(-3.36f, 0.19f);
    [SerializeField] private Vector2 fastGunMuzzleOffset = new Vector2(-3.12f, 0.37f);
    [SerializeField] private float slowGunSpeedMultiplier = 0.5f;
    [SerializeField] private float slowGunGravityMultiplier = 0.4f;
    [SerializeField] private float redFireRate = 0.2f;
    [SerializeField] private int blueGunMaxAmmo = 2;
    [SerializeField] private float blueGunReloadTime = 1f;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip jumpClip;
    [SerializeField] private AudioClip fastJumpClip;
    [SerializeField] private AudioClip blueGunReloadCompleteClip;
    [SerializeField] private AudioClip blueGunEmptyClip;

    private float defaultGravityScale;
    private bool isGrounded;
    private float lastRedFireTime = -999f;
    private int blueGunAmmo;
    private bool isBlueGunReloading;
    private float reloadTimer;

    public bool IsGrounded => isGrounded;
    public int BlueGunAmmo => blueGunAmmo;
    public int BlueGunMaxAmmo => blueGunMaxAmmo;
    public bool IsBlueGunReloading => isBlueGunReloading;

    public event System.Action<int, int> BlueAmmoChanged;

    public void ResetGravity()
    {
        rb.gravityScale = defaultGravityScale;
    }

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (weaponAim == null) weaponAim = GetComponentInChildren<WeaponAim>();
        if (playerHealth == null) playerHealth = GetComponent<PlayerHealth>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        defaultGravityScale = rb.gravityScale;
        blueGunAmmo = blueGunMaxAmmo;
    }

    private void Update()
    {
        if (isBlueGunReloading)
        {
            reloadTimer -= Time.deltaTime;
            if (reloadTimer <= 0f)
            {
                isBlueGunReloading = false;
                blueGunAmmo = blueGunMaxAmmo;
                BlueAmmoChanged?.Invoke(blueGunAmmo, blueGunMaxAmmo);
                if (audioSource != null && blueGunReloadCompleteClip != null) audioSource.PlayOneShot(blueGunReloadCompleteClip, AudioManager.SFXVolume);
            }
        }

        if (Mouse.current == null || weaponAim == null) return;

        bool leftHeld = Mouse.current.leftButton.isPressed;
        bool rightClicked = Mouse.current.rightButton.wasPressedThisFrame;
        bool leftReady = leftHeld && Time.time - lastRedFireTime >= redFireRate;

        if (!leftReady && !rightClicked) return;

        Vector2 jumpDirection = -weaponAim.AimDirection;

        if (leftReady)
        {
            lastRedFireTime = Time.time;
            float slowGravity = defaultGravityScale * slowGunGravityMultiplier;
            PerformJump(jumpForce * slowGunSpeedMultiplier, slowGunSprite, slowGunParticleColor, slowGunMuzzleOffset, jumpDirection, jumpClip, slowGravity);
        }
        else if (rightClicked)
        {
            if (isBlueGunReloading || blueGunAmmo <= 0)
            {
                if (audioSource != null && blueGunEmptyClip != null) audioSource.PlayOneShot(blueGunEmptyClip, AudioManager.SFXVolume);
                return;
            }

            blueGunAmmo--;
            BlueAmmoChanged?.Invoke(blueGunAmmo, blueGunMaxAmmo);
            PerformJump(fastJumpForce, fastGunSprite, fastGunParticleColor, fastGunMuzzleOffset, jumpDirection, fastJumpClip, defaultGravityScale);

            if (blueGunAmmo <= 0)
            {
                isBlueGunReloading = true;
                reloadTimer = blueGunReloadTime;
            }
        }
    }

    private void PerformJump(float force, Sprite gunSprite, Color particleColor, Vector2 muzzleOffset, Vector2 jumpDirection, AudioClip clip, float gravityScale)
    {
        if (audioSource != null && clip != null) audioSource.PlayOneShot(clip, AudioManager.SFXVolume);
        if (weaponAim.SpriteRenderer != null && gunSprite != null) weaponAim.SpriteRenderer.sprite = gunSprite;

        SpawnMuzzleEffect(particleColor, muzzleOffset, jumpDirection);

        rb.linearVelocity = jumpDirection * force;
        rb.gravityScale = gravityScale;

        isGrounded = false;
    }

    // The gun sprites point their barrel away from the cursor, so the effect fires along jumpDirection.
    // muzzleOffset is measured in the weapon's local space with the sprite unflipped.
    private void SpawnMuzzleEffect(Color color, Vector2 muzzleOffset, Vector2 barrelDirection)
    {
        if (muzzleEffectPrefab == null) return;

        SpriteRenderer gunRenderer = weaponAim.SpriteRenderer;
        if (gunRenderer != null && gunRenderer.flipX) muzzleOffset.y = -muzzleOffset.y;

        Vector3 position = weaponAim.transform.TransformPoint(muzzleOffset);
        Quaternion rotation = Quaternion.LookRotation(barrelDirection, Vector3.back);
        GameObject effect = Instantiate(muzzleEffectPrefab, position, rotation);

        // The impact prefab includes a wall bullet-hole decal, which makes no sense floating at the barrel.
        foreach (MeshRenderer decal in effect.GetComponentsInChildren<MeshRenderer>()) Destroy(decal.gameObject);

        foreach (ParticleSystem particles in effect.GetComponentsInChildren<ParticleSystem>())
        {
            var main = particles.main;
            Color tint = color;
            tint.a = main.startColor.color.a;
            main.startColor = tint;
        }

        if (gunRenderer == null) return;
        foreach (ParticleSystemRenderer particleRenderer in effect.GetComponentsInChildren<ParticleSystemRenderer>())
        {
            particleRenderer.sortingLayerID = gunRenderer.sortingLayerID;
            particleRenderer.sortingOrder = gunRenderer.sortingOrder + 1;
        }
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
