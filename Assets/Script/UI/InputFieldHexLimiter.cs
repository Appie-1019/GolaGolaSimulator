using TMPro;
using UnityEngine;

public class InputFieldHexLimiter : MonoBehaviour
{
    [HideInInspector] public TMP_InputField inputField;
    [HideInInspector] public Color currentColor;
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

        text.ToUpperInvariant();
        if (!text.StartsWith("#")) text = "#" + text;
        if (ColorUtility.TryParseHtmlString(text, out Color newColor))
        {
            inputField.SetTextWithoutNotify(text);
            currentColor = newColor;
            BackgroundManager.Instance?.UpdateBackgroundUI(BackgroundSetterType.HexInput);
        }
        else
        {
            inputField.SetTextWithoutNotify(lastText);
        }
    }

    public void SetHexFromColor(Color color)
    {
        currentColor = color;
        string hexString = ColorUtility.ToHtmlStringRGB(color);
        hexString = "#" + hexString;
        inputField.SetTextWithoutNotify(hexString);
        lastText = hexString;
    }
}
