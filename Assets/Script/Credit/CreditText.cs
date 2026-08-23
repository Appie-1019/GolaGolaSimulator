using TMPro;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CreditText : MonoBehaviour
{
    [SerializeField] private RectTransform rect;
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private float speed = 100f;

    private Vector3[] corners = new Vector3[4];

    [HideInInspector] public Canvas canvas;

    private void Awake()
    {
        CreditPrinter.allText.Add(this);
    }

    private void Start()
    {
        rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, 0f);
    }

    void Update()
    {
        rect.anchoredPosition += Vector2.up * speed * TimeManager.CustomDeltaTime;

        if (IsOutOfScreenTop(rect))
        {
            Destroy(gameObject);
        }
    }

    public void SetText(string text) => this.text.text = text;
    public void SetFontSize(float size) => text.fontSize = size;
    public void SetColor(Color color) => text.color = color;

    private bool IsOutOfScreenTop(RectTransform rect)
    {
        rect.GetWorldCorners(corners);
        Camera uiCamera = null;

        if (canvas != null && canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera != null ? canvas.worldCamera : Camera.main;
        }

        float bottomScreenY = uiCamera != null
            ? uiCamera.WorldToScreenPoint(corners[0]).y
            : corners[0].y;

        return bottomScreenY > Screen.height;
    }

    private void OnDestroy()
    {
        CreditPrinter.allText.Remove(this);
    }
}
