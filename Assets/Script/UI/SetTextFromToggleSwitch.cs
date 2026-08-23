using DG.Tweening;
using TMPro;
using UnityEngine;

public class SetTextFromToggleSwitch : MonoBehaviour
{
    public ToggleSwitch toggleSwitch;
    [Header("Color")]
    public Color enableColor = Color.green;
    public Color disableColor = Color.red;
    public float colorTransitionDuration = 0.2f;
    [Header("Text")]
    public string enableText = "Enabled";
    public string disableText = "Disabled";

    private TextMeshProUGUI tmp;
    private Tweener tween;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
    }

    private void Start()
    {
        if (tmp == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }

        toggleSwitch.AddToggleListener(OnToggleChanged);
        OnToggleChanged(toggleSwitch.isEnable);
    }

    private void OnToggleChanged(bool isOn)
    {
        if (tmp == null) return;

        Color newColor = isOn ? enableColor : disableColor;

        if (tween != null && tween.IsActive()) tween.Kill();
        tween = DOTween.To(
            () => tmp.color,
            x => tmp.color = x,
            newColor,
            colorTransitionDuration
        );

        tmp.text = isOn ? enableText : disableText;
    }
}
