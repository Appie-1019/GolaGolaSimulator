using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UISpreader : MonoBehaviour
{
    [Header("Start")]
    public bool closeAtStart = false;
    [Header("UI Spread")]
    public RectTransform UIRect;
    public float actDurtaion = 0.2f;
    [Header("Icon")]
    public Image iconImage;
    public Color openColor = Color.green;
    public Color closeColor = Color.red;
    [Header("Icon Animation")]
    public float pressedScale = 0.6f;
    public float reductionDuration = 0.15f;
    public float expansionDuration = 0.15f;

    RectTransform rect;
    bool isOpen = false;
    readonly Vector3 ClosedScale = new Vector3(1, 0, 1);
    readonly Vector3 ClosedRotation = new Vector3(0, 0, 179.999f);
    Vector3 originalSizeDelta;
    Vector3 originalScale;
    Vector3 closeSizeDelta;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (UIRect == null || iconImage == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
    }

    void Start()
    {
        originalSizeDelta = UIRect.sizeDelta;
        closeSizeDelta = UIRect.sizeDelta;
        closeSizeDelta.y = 0;
        originalScale = rect.localScale;
        SetState(isOpen, true);
    }

    public void Toggle()
    {
        SetState(!isOpen);
    }

    void SetState(bool enable, bool instant = false)
    {
        UIRect.DOKill();
        rect.DOKill();
        iconImage.DOKill();

        Vector3 targetScale = enable ? Vector3.one : ClosedScale;
        Vector3 targetSizeDelta = enable ? originalSizeDelta : closeSizeDelta;

        if (instant)
        {
            UIRect.localScale = targetScale;
            UIRect.sizeDelta = targetSizeDelta;
        }
        else
        {
            UIRect.DOScale(targetScale, actDurtaion).SetEase(Ease.OutQuad);
            UIRect.DOSizeDelta(targetSizeDelta, actDurtaion).SetEase(Ease.OutQuad);
        }
        
        Vector3 targetRotation = enable ? Vector3.zero : ClosedRotation;
        if (instant) rect.rotation = Quaternion.Euler(targetRotation);
        else rect.DORotate(targetRotation, actDurtaion).SetEase(Ease.OutQuad);

        Color targetColor = enable ? openColor : closeColor;
        if (instant) iconImage.color = targetColor;
        else iconImage.DOColor(targetColor, actDurtaion);

        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOScale(originalScale * pressedScale, reductionDuration));
        seq.Append(rect.DOScale(originalScale, expansionDuration));

        isOpen = enable;
    }
}
