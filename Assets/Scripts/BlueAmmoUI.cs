using UnityEngine;
using TMPro;

public class BlueAmmoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private PlayerRecoilJump recoilJump;

    private void Awake()
    {
        if (text == null) text = GetComponent<TextMeshProUGUI>();
    }

    private void OnEnable()
    {
        if (recoilJump == null) return;
        recoilJump.BlueAmmoChanged += UpdateText;
    }

    private void OnDisable()
    {
        if (recoilJump == null) return;
        recoilJump.BlueAmmoChanged -= UpdateText;
    }

    private void Start()
    {
        // Runs after every object's Awake has completed, unlike OnEnable
        // (whose cross-object ordering relative to other Awakes isn't guaranteed),
        // so PlayerRecoilJump's ammo fields are reliably initialized by this point.
        if (recoilJump == null) return;
        UpdateText(recoilJump.BlueGunAmmo, recoilJump.BlueGunMaxAmmo);
    }

    private void UpdateText(int current, int max)
    {
        text.text = current + "/" + max;
    }
}
