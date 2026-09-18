using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private PlayerRecoilJump recoilJump;

    private Vector3 checkpointPosition;

    private void Awake()
    {
        if (rb == null) rb = GetComponent<Rigidbody2D>();
        if (recoilJump == null) recoilJump = GetComponent<PlayerRecoilJump>();
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
    }
}
