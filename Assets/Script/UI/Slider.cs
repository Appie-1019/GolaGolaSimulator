using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slider : MonoBehaviour, IPointerDownHandler, IDragHandler
{
    [Header("UI")]
    [SerializeField] private RectTransform bgRect;
    [SerializeField] private RectTransform handleRect;
    [SerializeField] private Image fillImage;

    [Header("Slider Settings")]
    [SerializeField] private float minValue = 0f;
    [SerializeField] private float maxValue = 1f;
    [SerializeField] private bool isInteger = false;

    [Range(0f, 1f)]
    [SerializeField] private float rawPercent = 0.5f;

    private System.Action<float> onValueChanged;

    /// <summary>
    /// 슬라이더의 현재 값.
    /// 설정 시 <see cref="UpdateSliderUI"/> 및 이벤트 호출
    /// </summary>
    public float Value
    {
        get
        {
            float calculatedValue = Mathf.Lerp(minValue, maxValue, rawPercent);
            //return isInteger ? Mathf.Round(calculatedValue) : calculatedValue;
            return isInteger ? Mathf.CeilToInt(calculatedValue) : calculatedValue;
        }
        set
        {
            SetTextWithoutNotify(value);
            onValueChanged?.Invoke(Value);
        }
    }

    public float MinValue { get => minValue; }
    public float MaxValue { get => maxValue; }

    private void OnValidate()
    {
        if (minValue >= maxValue) maxValue = minValue + 0.001f;
        UpdateSliderUI();
    }

    private void Start()
    {
        UpdateSliderUI();
        onValueChanged?.Invoke(Value);
    }

    /// <summary> <see cref="IPointerClickHandler"/>에 종속됨. 클릭 시 <see cref="UpdateSliderFromPointer"/> 호출 </summary>
    /// <param name="eventData">포인터 이벤트 데이터</param>
    public void OnPointerDown(PointerEventData eventData)
    {
        UpdateSliderFromPointer(eventData);
    }

    /// <summary> <see cref="IDragHandler"/>에 종속됨. 드래그 시 <see cref="UpdateSliderFromPointer"/> 호출 </summary>
    /// <param name="eventData">포인터 이벤트 데이터</param>
    public void OnDrag(PointerEventData eventData)
    {
        UpdateSliderFromPointer(eventData);
    }

    /// <summary> <paramref name="eventData"/>를 기반으로 슬라이더 비율 계산 및 UI 갱신 </summary>
    /// <param name="eventData">포인터 이벤트 데이터</param>
    private void UpdateSliderFromPointer(PointerEventData eventData)
    {
        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(bgRect, eventData.position, eventData.pressEventCamera, out Vector2 localPoint))
        {
            float width = bgRect.rect.width;
            float adjustedX = localPoint.x + (width * bgRect.pivot.x);

            rawPercent = Mathf.Clamp01(adjustedX / width);

            UpdateSliderUI();
            onValueChanged?.Invoke(Value);
        }
    }

    /// <summary> 현재 비율을 바탕으로 슬라이더 핸들 위치 및 채우기 이미지 갱신 </summary>
    private void UpdateSliderUI()
    {
        if (bgRect == null) return;

        float width = bgRect.rect.width;
        float pivotOffset = width * bgRect.pivot.x;

        float displayPercent = rawPercent;
        //if (isInteger && maxValue != minValue) displayPercent = (Value - minValue) / (maxValue - minValue);

        if (handleRect != null)
        {
            float localX = Mathf.Lerp(-pivotOffset, width - pivotOffset, displayPercent);
            handleRect.anchoredPosition = new Vector2(localX, handleRect.anchoredPosition.y);
        }

        if (fillImage != null) fillImage.fillAmount = displayPercent;
    }

    /// <summary> 슬라이더의 값을 설정하지만 UI 갱신 및 이벤트 호출을 하지 않음 </summary>
    /// <param name="value">설정할 값</param>
    public void SetTextWithoutNotify(float value)
    {
        float clampedValue = Mathf.Clamp(value, minValue, maxValue);
        rawPercent = Mathf.InverseLerp(minValue, maxValue, clampedValue);

        UpdateSliderUI();
    }

    /// <summary> 값 변경 이벤트에 <paramref name="listener"/> 추가 </summary>
    /// <param name="listener">추가할 콜백 함수</param>
    public void AddListener(System.Action<float> listener) => onValueChanged += listener;
    /// <summary> 값 변경 이벤트에서 <paramref name="listener"/> 제거 </summary>
    /// <param name="listener">제거할 콜백 함수</param>
    public void RemoveListener(System.Action<float> listener) => onValueChanged -= listener;
}