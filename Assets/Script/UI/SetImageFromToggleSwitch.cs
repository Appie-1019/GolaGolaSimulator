using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class SetImageFromToggleSwitch : MonoBehaviour
{
    public ToggleSwitch toggleSwitch;
    [Header("Image")]
    public Sprite enableImage;
    public Sprite disableImage;
    [Header("Color")]
    public Color enableColor = Color.green;
    public Color disableColor = Color.red;
    public float colorTransitionDuration = 0.2f;

    private Tweener tween;
    Image img;

    private void Awake()
    {
        img = GetComponent<Image>();

        toggleSwitch.AddToggleListener(OnToggleChanged);
        OnToggleChanged(toggleSwitch.isEnable);
    }

    private void OnToggleChanged(bool isOn)
    {
        Color newColor = isOn ? enableColor : disableColor;

        if (tween != null && tween.IsActive()) tween.Kill();
        tween = DOTween.To(
            () => img.color,
            x => img.color = x,
            newColor,
            colorTransitionDuration
        );

        img.sprite = isOn ? enableImage : disableImage;
    }
}
