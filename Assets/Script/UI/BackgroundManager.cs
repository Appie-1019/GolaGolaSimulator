using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BackgroundSetterType
{
    Slider, InputField
}

public class BackgroundManager : MonoBehaviour
{
    public static BackgroundManager Instance { get; private set; }

    [Header("Setter")]
    public Slider[] sliders;
    public InputFieldValueLimiter[] inputFields;
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
    }

    public void OnSliderValueChanged(float uselessValue)
    {
        UpdateBackgroundUI(BackgroundSetterType.Slider);
    }

    public void UpdateBackgroundUI(BackgroundSetterType signalFrom)
    {
        if (signalFrom == BackgroundSetterType.Slider)
        {
            for (int i = 0; i < sliders.Length; i++)
            {
                if (inputFields[i] != null) inputFields[i].SetFloatToText(sliders[i].Value);
            }
        }
        else if (signalFrom == BackgroundSetterType.InputField)
        {
            for (int i = 0; i < inputFields.Length; i++)
            {
                if (sliders[i] != null) sliders[i].SetValueWithoutNotify(float.Parse(inputFields[i].inputField.text));
            }
        }

        Color newColor = new Color(sliders[0].Value, sliders[1].Value, sliders[2].Value);
        backgroundPreview.color = backgroundColor = Camera.main.backgroundColor = newColor;
    }

    private void SetColor(Color newColor)
    {
        inputFields[0].SetFloatToText(newColor.r); 
        inputFields[1].SetFloatToText(newColor.g); 
        inputFields[2].SetFloatToText(newColor.b);
        sliders[0].SetValueWithoutNotify(newColor.r);
        sliders[1].SetValueWithoutNotify(newColor.g);
        sliders[2].SetValueWithoutNotify(newColor.b);

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
