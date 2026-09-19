using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public float sizeIncrease = 1.2f;
    private Vector2 originalSize;
    private RectTransform rectTransform;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        originalSize = rectTransform.sizeDelta;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        rectTransform.sizeDelta = originalSize * sizeIncrease;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rectTransform.sizeDelta = originalSize;
    }


}
