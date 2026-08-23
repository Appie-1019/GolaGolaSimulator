using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ToggleSwitch : MonoBehaviour
{
    [Header("Color")]
    public Color enableColor;
    public Color disableColor;
    [Header("Sprite")]
    public RectTransform dot;
    public Image backgroundImage;
    public float moveDuration = 0.15f;
    [Header("Start")]
    public bool EnableAsStart = true;

    [HideInInspector] public bool isEnable;

    Vector2 dotPos;
    Button button;
    System.Action<bool> toggleListener;

    private void Awake()
    {
        if (dot != null)
        {
            dotPos = dot.anchoredPosition;
        }
        else
        {
            Debug.LogError(gameObject.name + "은(는) Dot가 없음.");
            this.enabled = false;
        }

        button = GetComponent<Button>();
        if (button != null)
        {
            button.onClick.AddListener(() => Toggle());
        }
        else
        {
            Debug.LogError(gameObject.name + "은(는) Button 컴포넌트가 필요함.");
            this.enabled = false;
        }

        if (backgroundImage == null)
        {
            Debug.LogError(gameObject.name + "은(는) backgroundImage가 없음.");
            this.enabled = false;
        }

        InitEnable(EnableAsStart);
    }

    /// <summary> 초기 <paramref name="enable"/> 상태 설정 및 UI 즉시 갱신 </summary>
    /// <param name="enable">활성화 여부</param>
    private void InitEnable(bool enable)
    {
        isEnable = enable;
        if (backgroundImage == null || dot == null) return;

        dot.DOKill();
        backgroundImage.DOKill();

        backgroundImage.color = enable ? enableColor : disableColor;
        dot.anchoredPosition = enable ? dotPos : -dotPos;
    }

    /// <summary> <see cref="isEnable"/> 상태를 반전시켜 <see cref="SetEnable(bool, bool)"/> 호출 </summary>
    /// <param name="instant">즉시 반영할지 여부</param>
    public void Toggle(bool instant = false) => SetEnable(!isEnable, instant);
    /// <summary> 지정된 <paramref name="enable"/> 상태로 스위치 변경 </summary>
    /// <param name="enable">활성화 여부</param>
    /// <param name="instant">즉시 반영할지 여부</param>
    public void SetEnable(bool enable, bool instant = false) => SetEnableState(enable, instant);
    /// <summary> 콜백 없이 <paramref name="enable"/> 상태로 스위치 변경 </summary>
    /// <param name="enable">활성화 여부</param>
    /// <param name="instant">즉시 반영할지 여부</param>
    public void SetEnableWithNoCallback(bool enable, bool instant = false) => SetEnableState(enable, instant, false);

    /// <summary> <paramref name="enable"/>로 토글 상태 변경, UI 애니메이션 처리 </summary>
    /// <param name="enable">활성화 여부</param>
    /// <param name="instant">즉시 반영할지 여부</param>
    /// <param name="hasCallback">콜백을 호출할지 여부</param>
    void SetEnableState(bool enable, bool instant, bool hasCallback = true)
    {
        if (enable == isEnable) return;
        isEnable = enable;
        if (backgroundImage == null || dot == null) return;

        dot.DOKill();
        backgroundImage.DOKill();
        Color targetColor = enable ? enableColor : disableColor;
        Vector2 targetPos = enable ? dotPos : -dotPos;

        if (instant)
        {
            dot.anchoredPosition = targetPos;
            backgroundImage.color = targetColor;
        }
        else
        {
            dot.DOAnchorPos(targetPos, moveDuration);
            backgroundImage.DOColor(targetColor, moveDuration);
        }

        if (hasCallback) toggleListener?.Invoke(isEnable);
    }

    /// <summary> 토글 상태 변경 이벤트에 <paramref name="listener"/> 추가 </summary>
    /// <param name="listener">추가할 콜백 함수</param>
    public void AddToggleListener(System.Action<bool> listener) => toggleListener += listener;
    /// <summary> 토글 상태 변경 이벤트에 <paramref name="listener"/> 제거</summary>
    /// <param name="listener">제거할 콜백 함수</param>
    public void RemoveToggleListener(System.Action<bool> listener) => toggleListener -= listener;
    private void OnDestroy() => toggleListener = null;
}