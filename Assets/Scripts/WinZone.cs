using System.Collections;
using UnityEngine;

// Put this on an object with a trigger BoxCollider2D (size 1x1) and stretch the object
// with its Transform scale to set the size of the win area.
[RequireComponent(typeof(BoxCollider2D))]
public class WinZone : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;
    [SerializeField] private AudioClip victoryClip;
    [SerializeField] private float winDelay = 1f;

    private AudioSource victorySource;
    private bool triggered;

    private void Awake()
    {
        if (gameManager == null) gameManager = FindFirstObjectByType<GameManager>();

        victorySource = GetComponent<AudioSource>();
        if (victorySource == null) victorySource = gameObject.AddComponent<AudioSource>();
        victorySource.playOnAwake = false;
        victorySource.spatialBlend = 0f;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered) return;

        var recoilJump = other.GetComponentInParent<PlayerRecoilJump>();
        if (recoilJump == null) return;
        triggered = true;

        // The win is locked in once the goal is reached, so nothing should be able to end the run before the panel shows.
        var health = recoilJump.GetComponent<PlayerHealth>();
        if (health != null) health.SetInvulnerable(true);

        StartCoroutine(ShowWinAfterDelay(recoilJump));
    }

    // Silences everything else (music, loops, effects) so only the victory clip is audible.
    // MusicPlayer resumes on its own once the scene reloads.
    private void PlayVictoryOnly(PlayerRecoilJump recoilJump)
    {
        if (victoryClip == null) return;

        foreach (var source in FindObjectsByType<AudioSource>(FindObjectsSortMode.None))
        {
            if (source != victorySource) source.Stop();
        }

        // The player can still shoot during the delay; mute its source so those shots stay silent.
        var playerAudio = recoilJump.GetComponent<AudioSource>();
        if (playerAudio != null) playerAudio.mute = true;

        victorySource.PlayOneShot(victoryClip, AudioManager.SFXVolume);
    }

    private IEnumerator ShowWinAfterDelay(PlayerRecoilJump recoilJump)
    {
        yield return new WaitForSeconds(winDelay);

        PlayVictoryOnly(recoilJump);

        recoilJump.enabled = false;
        var rb = recoilJump.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
        }

        if (gameManager != null) gameManager.ShowWin();
    }

    private void OnDrawGizmos()
    {
        var box = GetComponent<BoxCollider2D>();
        if (box == null) return;

        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = new Color(0.2f, 1f, 0.3f, 0.15f);
        Gizmos.DrawCube(box.offset, box.size);
        Gizmos.color = new Color(0.2f, 1f, 0.3f, 1f);
        Gizmos.DrawWireCube(box.offset, box.size);
    }
}
