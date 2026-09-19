using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class HeartsUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject[] hearts;
    [SerializeField] private float fadeDuration = 0.5f;

    private Image[] heartImages;
    private Coroutine[] fadeRoutines;

    private void Awake()
    {
        heartImages = new Image[hearts.Length];
        fadeRoutines = new Coroutine[hearts.Length];
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null) heartImages[i] = hearts[i].GetComponent<Image>();
        }
    }

    private void OnEnable()
    {
        if (playerHealth == null) return;
        playerHealth.LivesChanged += UpdateHearts;
    }

    private void OnDisable()
    {
        if (playerHealth == null) return;
        playerHealth.LivesChanged -= UpdateHearts;
    }

    private void Start()
    {
        // Runs after every object's Awake has completed, unlike OnEnable
        // (whose cross-object ordering relative to other Awakes isn't guaranteed),
        // so PlayerHealth.CurrentLives is reliably initialized by this point.
        if (playerHealth == null) return;
        SnapHearts(playerHealth.CurrentLives);
    }

    // Sets the starting state instantly -- only lives lost during actual play should fade.
    private void SnapHearts(int current)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null || heartImages[i] == null) continue;
            hearts[i].SetActive(true);
            SetAlpha(heartImages[i], i < current ? 1f : 0f);
        }
    }

    private void UpdateHearts(int current, int max)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] == null || heartImages[i] == null) continue;

            float targetAlpha = i < current ? 1f : 0f;
            if (fadeRoutines[i] != null) StopCoroutine(fadeRoutines[i]);
            fadeRoutines[i] = StartCoroutine(FadeHeart(i, targetAlpha));
        }
    }

    private IEnumerator FadeHeart(int index, float targetAlpha)
    {
        var image = heartImages[index];
        float startAlpha = image.color.a;
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            SetAlpha(image, Mathf.Lerp(startAlpha, targetAlpha, elapsed / fadeDuration));
            yield return null;
        }

        SetAlpha(image, targetAlpha);
        fadeRoutines[index] = null;
    }

    private static void SetAlpha(Image image, float alpha)
    {
        var color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
