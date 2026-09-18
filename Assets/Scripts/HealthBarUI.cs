using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [SerializeField] private Slider slider;
    [SerializeField] private PlayerHealth playerHealth;

    private void Awake()
    {
        if (slider == null) slider = GetComponent<Slider>();
    }

    private void OnEnable()
    {
        if (playerHealth == null) return;
        playerHealth.HealthChanged += UpdateBar;
        UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void OnDisable()
    {
        if (playerHealth == null) return;
        playerHealth.HealthChanged -= UpdateBar;
    }

    private void UpdateBar(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
