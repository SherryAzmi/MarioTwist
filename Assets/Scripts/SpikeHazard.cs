using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    // All spikes share one tilemap collider, so OnTriggerEnter2D only fires the first time the player
    // touches any spike. OnTriggerStay2D keeps checking while the player overlaps them, so a hit is
    // never lost (e.g. touching a spike during respawn protection, then staying in it once it ends).
    // LoseHeart already ignores calls while the player is invulnerable and respawns them away otherwise.
    private void OnTriggerEnter2D(Collider2D other)
    {
        Hurt(other);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        Hurt(other);
    }

    private static void Hurt(Collider2D other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null) playerHealth.LoseHeart();
    }
}
