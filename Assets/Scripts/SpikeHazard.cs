using UnityEngine;

public class SpikeHazard : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        var playerHealth = other.GetComponent<PlayerHealth>();
        if (playerHealth != null) playerHealth.Kill();
    }
}
