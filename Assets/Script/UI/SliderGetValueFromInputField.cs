using TMPro;
using UnityEngine;

public class SliderGetValueFromInputField : MonoBehaviour
{
    public TMP_InputField inputField;
    private Slider slider;

    private void Awake()
    {
        slider = GetComponent<Slider>();
        if (slider == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
    }

    private void Start()
    {
        if (inputField == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
        
        inputField.onValueChanged.AddListener(SetValueFromText);
    }

    private void SetValueFromText(string text)
    {
        if (float.TryParse(text, out float value))
        {
            slider.SetTextWithoutNotify(value);
        }
    }
}
