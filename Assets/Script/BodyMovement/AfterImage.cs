using UnityEngine;

public class AfterImage : MonoBehaviour
{
    public SpriteRenderer head;
    public SpriteRenderer head_out;
    public SpriteRenderer body;
    public SpriteRenderer body_out;

    public SpriteRenderer[] allRenderer;
    public float duration;
    public Vector3 targetScale;

    private float elapsedTime = 0f;
    private Vector3 initialScale;

    private void Start()
    {
        if (SkinManager.Instance == null)
        {
            Destroy(gameObject);
            return;
        }

        head.sprite = SkinManager.Instance.head.sprite;
        head_out.sprite = SkinManager.Instance.head_out.sprite;
        body.sprite = SkinManager.Instance.body.sprite;
        body_out.sprite = SkinManager.Instance.body_out.sprite;

        initialScale = transform.localScale;
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;

        float progress = Mathf.Clamp01(elapsedTime / duration);
        float alpha = 1f - progress;

        for (int i = 0; i < allRenderer.Length; i++)
        {
            Color col = allRenderer[i].color;
            col.a = alpha;
            allRenderer[i].color = col;
        }

        transform.localScale = Vector3.Lerp(initialScale, targetScale, progress);

        if (elapsedTime >= duration)
        {
            Destroy(gameObject);
        }
    }
}
