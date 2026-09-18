using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponAim : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public Vector2 AimDirection { get; private set; } = Vector2.right;
    public SpriteRenderer SpriteRenderer => spriteRenderer;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
        if (spriteRenderer == null) spriteRenderer = GetComponentInChildren<SpriteRenderer>();
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

        if (spriteRenderer != null)
        {
            bool facingLeft = angle > 90f || angle < -90f;
            spriteRenderer.flipY = facingLeft;
        }
    }
}
