using UnityEngine;

public class HeartsUI : MonoBehaviour
{
    [SerializeField] private PlayerHealth playerHealth;
    [SerializeField] private GameObject[] hearts;

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
        UpdateHearts(playerHealth.CurrentLives, playerHealth.MaxLives);
    }

    private void UpdateHearts(int current, int max)
    {
        for (int i = 0; i < hearts.Length; i++)
        {
            if (hearts[i] != null) hearts[i].SetActive(i < current);
        }
    }
}
