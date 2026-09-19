using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

// The intro cutscene: fades in from black, types the text one character at a time like someone typing it,
// holds for a moment, fades the text out, then loads the next scene.
public class TypewriterCutscene : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private ScreenFader fader;
    [SerializeField] private AudioSource typingSound;   // optional looping typing sound: plays only while the text is being typed
    [SerializeField] private string nextScene = "CoreScene";
    [SerializeField] private float fadeInDuration = 0.8f;
    [SerializeField] private float charactersPerSecond = 28f;
    [SerializeField] private float pauseAfterSentence = 0.35f;   // extra pause after . ! ? and line breaks
    [SerializeField] private float holdAfterTyping = 1.5f;
    [SerializeField] private float textFadeDuration = 1f;

    private IEnumerator Start()
    {
        Time.timeScale = 1f;

        // Lay out the full text first (so lines wrap correctly), then reveal it character by character.
        text.ForceMeshUpdate();
        int total = text.textInfo.characterCount;
        text.maxVisibleCharacters = 0;
        text.alpha = 1f;

        if (fader != null) yield return fader.FadeTo(0f, fadeInDuration);

        float delay = 1f / Mathf.Max(1f, charactersPerSecond);
        if (typingSound != null) typingSound.Play();
        for (int i = 1; i <= total; i++)
        {
            text.maxVisibleCharacters = i;

            char typed = text.textInfo.characterInfo[i - 1].character;
            float wait = delay;
            if (typed == '.' || typed == '!' || typed == '?' || typed == '\n') wait += pauseAfterSentence;
            yield return new WaitForSecondsRealtime(wait);
        }

        if (typingSound != null) typingSound.Stop();   // typing is done: silence it right away

        yield return new WaitForSecondsRealtime(holdAfterTyping);

        float elapsed = 0f;
        while (elapsed < textFadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            text.alpha = Mathf.Lerp(1f, 0f, elapsed / textFadeDuration);
            yield return null;
        }
        text.alpha = 0f;

        SceneManager.LoadScene(nextScene);
    }
}
