using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Checkpoint : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var respawn = other.GetComponent<PlayerRespawn>();
        if (respawn != null) respawn.SetCheckpoint(transform.position);
    }
}
