using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeMagnitude = 0.15f;
    [SerializeField] private float maxContinuousShakeDuration = 4f;
    [SerializeField] private float sessionGapThreshold = 1f;

    private Vector3 originalLocalPosition;
    private Coroutine shakeRoutine;
    private float lastShakeCallTime = -999f;
    private float sessionStartTime = -999f;

    private void Awake()
    {
        originalLocalPosition = transform.localPosition;
    }

    public void Shake()
    {
        // Repeated hits (e.g. standing in a damage-over-time hazard) call Shake()
        // faster than any single shake lasts, so a "session" is a run of calls with
        // no gap longer than sessionGapThreshold between them, not just one shake's
        // own duration. Once a session has run for maxContinuousShakeDuration, further
        // calls in it are ignored until a real gap starts a fresh session.
        float now = Time.time;
        if (now - lastShakeCallTime > sessionGapThreshold) sessionStartTime = now;
        lastShakeCallTime = now;

        if (now - sessionStartTime >= maxContinuousShakeDuration) return;

        if (shakeRoutine != null) StopCoroutine(shakeRoutine);
        shakeRoutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;
        while (elapsed < shakeDuration)
        {
            Vector2 offset = Random.insideUnitCircle * shakeMagnitude;
            transform.localPosition = originalLocalPosition + (Vector3)offset;
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalLocalPosition;
        shakeRoutine = null;
    }
}
