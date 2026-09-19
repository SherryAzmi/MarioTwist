using UnityEngine;

// Keeps a background sprite filling the whole orthographic camera view at any aspect ratio.
// Make this object a child of the camera: it then follows the player (and the camera shake)
// for free, and this script only sizes it and holds it a fixed distance in front of the camera.
[RequireComponent(typeof(SpriteRenderer))]
public class CameraBackground : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float distanceFromCamera = 10f;
    [SerializeField] private float overscan = 1.02f;

    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (cam == null) cam = GetComponentInParent<Camera>();
        if (cam == null) cam = Camera.main;
    }

    private void LateUpdate()
    {
        if (cam == null || !cam.orthographic || spriteRenderer.sprite == null) return;

        float viewHeight = cam.orthographicSize * 2f;
        float viewWidth = viewHeight * cam.aspect;
        Vector2 spriteSize = spriteRenderer.sprite.bounds.size;
        float worldScale = Mathf.Max(viewWidth / spriteSize.x, viewHeight / spriteSize.y) * overscan;

        // The camera is a child of the (scaled) player, so undo the parent's scale to get the wanted world size.
        Vector3 parentScale = transform.parent != null ? transform.parent.lossyScale : Vector3.one;
        transform.localScale = new Vector3(worldScale / parentScale.x, worldScale / parentScale.y, 1f);
        transform.localPosition = new Vector3(0f, 0f, distanceFromCamera / parentScale.z);
    }
}
