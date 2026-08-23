using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public SpriteRenderer headRenderer;
    public SpriteRenderer bodyRenderer;
    public float duration;
    public Vector3 targetScale;

    private float elapsedTime = 0f;
    private Color initialHeadColor;
    private Color initialBodyColor;
    private Vector3 initialScale;

    private void Start()
    {
        if (headRenderer != null)
            initialHeadColor = headRenderer.color;

        if (bodyRenderer != null)
            initialBodyColor = bodyRenderer.color;

        initialScale = transform.localScale;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float progress = Mathf.Clamp01(elapsedTime / duration);
        float alpha = 1f - progress;

        if (headRenderer != null)
        {
            initialHeadColor.a = alpha;
            headRenderer.color = initialHeadColor;
        }

        if (bodyRenderer != null)
        {
            initialBodyColor.a = alpha;
            bodyRenderer.color = initialBodyColor;
        }

        transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);

        if (elapsedTime >= duration)
        {
            Destroy(gameObject);
        }
    }
}
