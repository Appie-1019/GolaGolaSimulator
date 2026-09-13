using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class ButtonInteractionMotion : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static Vector2 sizeMultiplier = new Vector2(1.2f, 1.2f);
    public static float actionDuration = 0.3f;

    RectTransform rect;
    private Vector2 ogSize;
    private Vector2 multiplierSize;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        ogSize = rect.localScale;
        multiplierSize = ogSize * sizeMultiplier;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        rect.DOKill();
        rect.DOScale(multiplierSize, actionDuration).SetEase(Ease.OutQuint);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        rect.DOKill();
        rect.DOScale(ogSize, actionDuration).SetEase(Ease.OutCirc);
    }

}
