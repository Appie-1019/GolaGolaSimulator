using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomBox : MonoBehaviour
{
    [SerializeField] private Color selectedColor;
    [SerializeField] private Color confirmedColor;

    [HideInInspector] public int selectedCount = 0;
    private Vector3 currentScale;
    private SpriteRenderer sr;
    private Color ogColor;

    private void Awake()
    {
        currentScale = transform.localScale;
        sr = GetComponent<SpriteRenderer>();
        ogColor = sr.color;
        selectedCount = 0;
    }

    public void AddScaleY(float value)
    {
        currentScale.y += value;
        transform.localScale = currentScale;
        sr.color = confirmedColor;
        selectedCount++;
    }

    public void ResetColor() => sr.color = ogColor;
    public void SetColorSelected() => sr.color = selectedColor;
}
