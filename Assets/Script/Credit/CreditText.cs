using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CreditText : MonoBehaviour
{
    public RectTransform rect;
    [SerializeField] private TextMeshProUGUI text;

    public void SetText(string text) => this.text.text = text;
    public void SetFontSize(float size) => text.fontSize = size;
    public void SetColor(Color color) => text.color = color;
}
