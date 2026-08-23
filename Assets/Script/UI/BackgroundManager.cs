using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BackgroundSetterType
{
    None, Slider, FloatInputField, HexInput
}

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance { get; private set; }

    [Header("Setter")]
    public Slider[] sliders;
    public InputFieldValueLimiter[] inputFieldsFloat;
    public InputFieldHexLimiter inputFieldsHex;
    [Header("BackgroundPreview")]
    public Image backgroundPreview;

    [HideInInspector] public Color backgroundColor = Color.white;

    Coroutine rainbowCoroutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        foreach (var slider in sliders)
        {
            slider.AddListener(OnSliderValueChanged);
        }

        SetColor(Color.white);
    }

    public void OnSliderValueChanged(float uselessValue)
    {
        UpdateBackgroundUI(BackgroundSetterType.Slider);
    }

    public void UpdateBackgroundUI(BackgroundSetterType signalFrom)
    {
        Color newColor = Color.white;

        if (signalFrom == BackgroundSetterType.Slider)
        {
            if (sliders[0] != null) newColor.r = sliders[0].Value;
            if (sliders[1] != null) newColor.g = sliders[1].Value;
            if (sliders[2] != null) newColor.b = sliders[2].Value;
        }
        else if (signalFrom == BackgroundSetterType.FloatInputField)
        {
            newColor.r = float.Parse(inputFieldsFloat[0]?.inputField.text);
            newColor.g = float.Parse(inputFieldsFloat[1]?.inputField.text);
            newColor.b = float.Parse(inputFieldsFloat[2]?.inputField.text);

        }
        else if (signalFrom  == BackgroundSetterType.HexInput)
        {
            newColor = inputFieldsHex.currentColor;
        }

        SetColor(newColor, signalFrom);
    }

    private void SetColor(Color newColor, BackgroundSetterType signalFrom = BackgroundSetterType.None)
    {
        if (signalFrom != BackgroundSetterType.FloatInputField)
        {
            inputFieldsFloat[0]?.SetFloatToText(newColor.r);
            inputFieldsFloat[1]?.SetFloatToText(newColor.g);
            inputFieldsFloat[2]?.SetFloatToText(newColor.b);
        }
        if (signalFrom != BackgroundSetterType.Slider)
        {
            sliders[0]?.SetValueWithoutNotify(newColor.r);
            sliders[1]?.SetValueWithoutNotify(newColor.g);
            sliders[2]?.SetValueWithoutNotify(newColor.b);
        }
        if (signalFrom != BackgroundSetterType.HexInput)
        {
            inputFieldsHex.SetHexFromColor(newColor);
        }

        backgroundPreview.color = backgroundColor = Camera.main.backgroundColor = newColor;
    }

    public void DoARainbow()
    {
        if (rainbowCoroutine != null) StopCoroutine(rainbowCoroutine);
        rainbowCoroutine = StartCoroutine(RainbowBackGround());
    }

    public void StopARainbow()
    {
        if (rainbowCoroutine != null)
        {
            StopCoroutine(rainbowCoroutine);
            rainbowCoroutine = null;
        }

        SetColor(Color.white);
    }

    IEnumerator RainbowBackGround()
    {
        float hue = 0f;

        while (true)
        {
            hue += Time.deltaTime;
            if (hue > 1f) hue -= 1f;

            Color nextColor = Color.HSVToRGB(hue, 1f, 1f);
            SetColor(nextColor);

            yield return null;
        }
    }

}
