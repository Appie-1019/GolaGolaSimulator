using TMPro;
using UnityEngine;

public class InputFieldGetValueFromSlider : MonoBehaviour
{
    public Slider slider;
    private TMP_InputField inputField;

    private string lastText = string.Empty;
    private System.Action onValueChanged;

    private void Awake()
    {
        inputField = GetComponent<TMP_InputField>();
        if (inputField == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
        inputField.onValueChanged.AddListener(OnValueChanged);
    }

    void Start()
    {
        if (slider == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
        slider.AddListener(SetFloatToText);
        SetFloatToText(slider.Value);
    }

    private void SetFloatToText(float value)
    {
        string text = value.ToString("0.0###");
        lastText = text;
        inputField.SetTextWithoutNotify(text);
    }

    private void OnValueChanged(string text)
    {
        if (string.IsNullOrEmpty(lastText) || !float.TryParse(text, out float value))
        {
            inputField.SetTextWithoutNotify(lastText);
        }
        else
        {
            value = Mathf.Clamp(value, slider.MinValue, slider.MaxValue);
            SetFloatToText(value);
            onValueChanged.Invoke();
        }
    }

    public void AddListener(System.Action action)
    {
        onValueChanged += action;
    }
}