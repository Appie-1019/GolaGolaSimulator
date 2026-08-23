using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(RectTransform))]
public class ButonPressAnimation : MonoBehaviour
{
    public float pressedScale = 0.6f;
    public float reductionDuration = 0.15f;
    public float expansionDuration = 0.15f;

    Button button;
    RectTransform rect;
    Vector3 originalScale;

    private void Awake()
    {
        button = GetComponent<Button>();
        rect = GetComponent<RectTransform>();

        originalScale = rect.localScale;
        button.onClick.AddListener(PlayButtonEffect);
    }

    /// <summary> 버튼 클릭 시 축소 및 확대 애니메이션 실행. </summary>
    public void PlayButtonEffect()
    {
        rect.DOKill();

        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOScale(originalScale * pressedScale, reductionDuration));
        seq.Append(rect.DOScale(originalScale, expansionDuration));
    }
}