using TMPro;
using UnityEngine;

public class InputFieldValueLimiter : MonoBehaviour
{
    public Slider slider;

    [HideInInspector] public TMP_InputField inputField;
    private string lastText = string.Empty;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        if (inputField == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
        inputField.onEndEdit.AddListener(CheckValue);
    }

    public void CheckValue(string text = "")
    {
        if (string.IsNullOrEmpty(text)) text = inputField.text;

        if (string.IsNullOrEmpty(lastText) || !float.TryParse(text, out float value))
        {
            inputField.SetTextWithoutNotify(lastText);
        }
        else
        {
            value = Mathf.Clamp(value, slider.MinValue, slider.MaxValue);
            SetFloatToText(value);
            BackgroundManager.Instance?.UpdateBackgroundUI(BackgroundSetterType.FloatInputField);
        }
    }

    public void SetFloatToText(float value)
    {
        string text = value.ToString("0.0###");
        lastText = text;
        inputField.SetTextWithoutNotify(text);
    }
}