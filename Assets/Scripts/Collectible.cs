using UnityEngine;

// A pickup (the stars). When the player touches it: play the pickup sound, play the effect that is a child of
// the star, and remove the star. The effect is detached first so it keeps playing after the star is gone.
public class Collectible : MonoBehaviour
{
    [SerializeField] private AudioClip collectSound;
    [SerializeField] private GameObject collectEffect;
    [SerializeField] private float effectLifetime = 3f;

    private bool collected;

    private void Awake()
    {
        // The effect must only play when the star is collected, not when the level starts.
        if (collectEffect != null) collectEffect.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (collected) return;

        var player = other.GetComponentInParent<PlayerRecoilJump>();
        if (player == null) return;
        collected = true;

        var playerAudio = player.GetComponent<AudioSource>();
        if (playerAudio != null && collectSound != null) playerAudio.PlayOneShot(collectSound, AudioManager.SFXVolume);

        if (collectEffect != null)
        {
            collectEffect.transform.SetParent(null, true);   // keep its world position and size
            collectEffect.SetActive(true);
            Destroy(collectEffect, effectLifetime);
        }

        Destroy(gameObject);
    }
}
