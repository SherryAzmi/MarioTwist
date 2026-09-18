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
        
    }

    private void OnDisable()
    {
        if (playerHealth == null) return;
        playerHealth.HealthChanged -= UpdateBar;
    }

    private void Start()
    {
        // Runs after every object's Awake has completed, unlike OnEnable
        // (whose cross-object ordering relative to other Awakes isn't guaranteed),
        // so PlayerHealth.CurrentHealth is reliably initialized by this point if the damage is .
        if (playerHealth == null) return;
        UpdateBar(playerHealth.CurrentHealth, playerHealth.MaxHealth);
    }

    private void UpdateBar(int current, int max)
    {
        slider.maxValue = max;
        slider.value = current;
    }
}
