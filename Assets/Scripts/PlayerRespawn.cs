using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerRecoilJump recoilJump;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip checkpointClip;

    private Vector3 checkpointPosition;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (recoilJump == null) recoilJump = GetComponent<PlayerRecoilJump>();
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        checkpointPosition = transform.position;
    }

    public void SetCheckpoint(Vector3 position)
    {
        checkpointPosition = position;
    }

    public void RespawnAtCheckpoint()
    {
        transform.position = checkpointPosition;
        if (rb != null) rb.linearVelocity = Vector2.zero;
        if (recoilJump != null) recoilJump.ResetGravity();
        if (audioSource != null && checkpointClip != null) audioSource.PlayOneShot(checkpointClip, AudioManager.SFXVolume);
    }
}
