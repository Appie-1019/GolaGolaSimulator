using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class UISpreader : MonoBehaviour
{
    [Header("Setting")]
    public bool OpenAtStart = false;
    public float clickCooldown = 0.4f;
    [Header("UI Spread")]
    public RectTransform UIRect;
    public float actDurtaion = 0.2f;
    [Header("Icon")]
    public Image iconImage;
    public Color openColor = Color.green;
    public Color closeColor = Color.red;
    [Header("Icon Animation")]
    public float pressedScale = 0.6f;
    public float reductionDuration = 0.15f;
    public float expansionDuration = 0.15f;
    [Header("Child object")]
    public UISpreader[] ChildSpreaders;
    public RectTransform[] extraReducedObject;

    RectTransform rect;
    bool isOpen = false;
    bool lastState = false;
    readonly Vector3 ClosedScale = new Vector3(1, 0, 1);
    readonly Vector3 ClosedRotation = new Vector3(0, 0, 179.999f);
    Vector3 originalSizeDelta;
    Vector3 originalScale;
    Vector3 closeSizeDelta;
    float[] extraReducedObjectHight;
    float lastClickTime;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        if (UIRect == null || iconImage == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
    }

    void Start()
    {
        originalSizeDelta = UIRect.sizeDelta;
        closeSizeDelta = UIRect.sizeDelta;
        closeSizeDelta.y = 0;
        originalScale = rect.localScale;
        CollectExtraHeights();

        SetState(OpenAtStart, true);
        lastState = OpenAtStart;
    }

    public void Toggle()
    {
        if (Time.time - lastClickTime >= clickCooldown)
        {
            lastClickTime = Time.time;

            lastState = !isOpen;
            SetState(lastState);
        }
    }

    void SetState(bool enable, bool instant = false, float timeReduction = 0)
    {
        UIRect.DOKill();
        rect.DOKill();
        iconImage.DOKill();

        Vector3 targetScale;
        Vector3 targetRotation;
        Color targetColor;
        Vector3 targetSizeDelta;

        if (enable)
        {
            targetScale = Vector3.one;
            targetRotation = Vector3.zero;
            targetColor = openColor;
            targetSizeDelta = originalSizeDelta;
            RestoreExtraObjectsHeight(instant);

        }
        else
        {
            targetScale = ClosedScale;
            targetRotation = ClosedRotation;
            targetColor = closeColor;
            targetSizeDelta = closeSizeDelta;
            SetExtraObjectsHeightsToZero(instant, 0.1f);
        }

        StartCoroutine(ForceChildContorl(enable));


        if (instant)
        {
            UIRect.localScale = targetScale;
            UIRect.sizeDelta = targetSizeDelta;
            
            rect.rotation = Quaternion.Euler(targetRotation);

            iconImage.color = targetColor;
        }
        else
        {
            float currentActDuration = actDurtaion - timeReduction;
            UIRect.DOScale(targetScale, currentActDuration).SetEase(Ease.OutQuad);
            UIRect.DOSizeDelta(targetSizeDelta, currentActDuration).SetEase(Ease.OutQuad).OnComplete(() => { UIRect.sizeDelta += Vector2.one * 0.005f; });

            rect.DORotate(targetRotation, currentActDuration).SetEase(Ease.OutQuad);

            iconImage.DOColor(targetColor, currentActDuration);
        }
        
        Sequence seq = DOTween.Sequence();
        seq.Append(rect.DOScale(originalScale * pressedScale, reductionDuration));
        seq.Append(rect.DOScale(originalScale, expansionDuration));

        isOpen = enable;
    }

    IEnumerator ForceChildContorl(bool isOpen)
    {
        foreach (UISpreader sp in ChildSpreaders)
        {
            if (isOpen)
            {
                yield return new WaitForSeconds(0.075f);
                sp.ForceOpen();
            }
            else sp.ForceClose();
        }
    }

    public void ForceOpen()
    {
        if (lastState != isOpen)
        {
            SetState(lastState);
            lastClickTime = Time.time;
        }
    }
    public void ForceClose()
    {
        if (lastState)
        {
            SetState(false);
            lastClickTime = Time.time;
        }
    }

    void CollectExtraHeights()
    {
        if (extraReducedObject == null || extraReducedObject.Length == 0)
        {
            extraReducedObjectHight = new float[0];
            return;
        }

        extraReducedObjectHight = new float[extraReducedObject.Length];

        for (int i = 0; i < extraReducedObject.Length; i++)
        {
            if (extraReducedObject[i] != null)
            {
                extraReducedObjectHight[i] = extraReducedObject[i].sizeDelta.y;
            }
            else
            {
                extraReducedObjectHight[i] = 0f;
            }
        }
    }

    public void SetExtraObjectsHeightsToZero(bool instant = false, float timeReduction = 0)
    {
        if (extraReducedObject == null) return;

        for (int i = 0; i < extraReducedObject.Length; i++)
        {
            if (extraReducedObject[i] != null)
            {
                Vector2 currentSize = extraReducedObject[i].sizeDelta;
                currentSize.y = 0f;
                if (instant) extraReducedObject[i].sizeDelta = currentSize;
                else extraReducedObject[i].DOSizeDelta(currentSize, actDurtaion - timeReduction);
            }
        }
    }

    public void RestoreExtraObjectsHeight(bool instant = false, float timeReduction = 0)
    {
        if (extraReducedObject == null || extraReducedObjectHight == null) return;

        int loopCount = Mathf.Min(extraReducedObject.Length, extraReducedObjectHight.Length);

        for (int i = 0; i < loopCount; i++)
        {
            if (extraReducedObject[i] != null)
            {
                Vector2 currentSize = extraReducedObject[i].sizeDelta;
                currentSize.y = extraReducedObjectHight[i];
                if (instant) extraReducedObject[i].sizeDelta = currentSize;
                else extraReducedObject[i].DOSizeDelta(currentSize, actDurtaion - timeReduction);
            }
        }
    }
}
