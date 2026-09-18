using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickSound : MonoBehaviour
{
    [SerializeField] private AudioClip clickClip;

    // Shared across every button so one AudioSource plays the click, not a duplicate
    // per button; self-heals across scene loads since a destroyed source reads as null.
    private static AudioSource sharedAudioSource;

    private void Awake()
    {
        if (sharedAudioSource == null)
        {
            var go = new GameObject("ButtonClickAudio");
            sharedAudioSource = go.AddComponent<AudioSource>();
            sharedAudioSource.playOnAwake = false;
        }

        GetComponent<Button>().onClick.AddListener(PlayClick);
    }

    private void PlayClick()
    {
        if (clickClip != null) sharedAudioSource.PlayOneShot(clickClip, AudioManager.SFXVolume);
    }
}
