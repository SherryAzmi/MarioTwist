using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer playerSpriteRenderer;

    public Vector2 AimDirection { get; private set; } = Vector2.right;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (playerSpriteRenderer == null) playerSpriteRenderer = GetComponentInParent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Mouse.current == null) return;

        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = cam.ScreenToWorldPoint(new Vector3(mouseScreenPos.x, mouseScreenPos.y, -cam.transform.position.z));

        Vector2 direction = (Vector2)mouseWorldPos - (Vector2)transform.position;
        AimDirection = direction.normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        transform.rotation = Quaternion.Euler(0f, 0f, angle);

        bool facingLeft = angle > 90f || angle < -90f;

        if (spriteRenderer != null)
        {
            // The gun art is drawn barrel-up with the grip on its left. It sits on the cursor side, rotated so the
            // barrel points at the cursor, so it must be mirrored when aiming right to keep the grip below the barrel.
            spriteRenderer.flipX = !facingLeft;
        }

        if (playerSpriteRenderer != null)
        {
            // The player art (idle and jump) is drawn facing left, so mirror it when aiming right to face the gun.
            playerSpriteRenderer.flipX = !facingLeft;
        }
    }
}
