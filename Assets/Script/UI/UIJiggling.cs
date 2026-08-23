using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIJiggling : MonoBehaviour
{
    RectTransform rect;

    Vector2 ogHight;
    Vector2 jigglingHight;
    bool tick;

    private void Start()
    {
        rect = GetComponent<RectTransform>();
        ogHight = rect.sizeDelta;
        jigglingHight = ogHight;
        jigglingHight.y += 0.005f;
    }

    void Update()
    {
        if (tick)
        {
            rect.sizeDelta = ogHight;
        }
        else
        {
            rect.sizeDelta = jigglingHight;
        }

        tick = !tick;
    }
}
