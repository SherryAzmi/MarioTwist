using System.Collections;
using UnityEngine;

// A full-screen black overlay (put it on a Canvas object with a black Image and a CanvasGroup) used to fade
// the screen to black and back. It uses unscaled time, so it also works while the game is paused (time = 0).
[RequireComponent(typeof(CanvasGroup))]
public class ScreenFader : MonoBehaviour
{
    private CanvasGroup group;

    private void Awake()
    {
        group = GetComponent<CanvasGroup>();
    }

    public IEnumerator FadeTo(float targetAlpha, float duration)
    {
        if (group == null) group = GetComponent<CanvasGroup>();

        group.blocksRaycasts = true;   // no clicks while the screen is fading
        float startAlpha = group.alpha;
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsed / duration);
            yield return null;
        }

        group.alpha = targetAlpha;
        group.blocksRaycasts = targetAlpha > 0f;
    }
}
