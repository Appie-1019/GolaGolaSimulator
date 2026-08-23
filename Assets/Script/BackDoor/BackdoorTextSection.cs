using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshPro))]
public class BackdoorTextSection : MonoBehaviour
{
    public TextMeshPro TMP;
    private Vector3 ogSize;
    private Vector3 ogPos;
    private BackdoorTextDataSection data;
    Coroutine appearCoroutine;

    private void Awake()
    {
        if (TMP == null) TMP = GetComponent<TextMeshPro>();
        ogSize = transform.localScale;
    }

    public void Init(BackdoorTextDataSection newData)
    {
        data = newData;
        TMP.color = data.mainColor;
        transform.localScale = ogSize;
        ogPos = transform.localPosition;
        StopAllCoroutines();
        appearCoroutine = null;

        if (data.appear != TextSectionAppear.None)
        {
            switch (data.appear)
            {
                case TextSectionAppear.Fade:
                    appearCoroutine = StartCoroutine(Appear_Fade());
                    break;
                case TextSectionAppear.Expansion:
                    appearCoroutine = StartCoroutine(Appear_Expansion());
                    break;
            }
        }

        if (data.idle != TextSectionIdle.None)
        {
            switch (data.idle)
            {
                case TextSectionIdle.Vibration:
                    StartCoroutine(Idle_Vibration());
                    break;
                case TextSectionIdle.Wobble:
                    StartCoroutine(Idle_Wobble());
                    break;
                case TextSectionIdle.Bobbing:
                    StartCoroutine(Idle_Bobbing());
                    break;
            }
        }

        if (data.color != TextSectionColor.None)
        {
            switch (data.color)
            {
                case TextSectionColor.Fade:
                    StartCoroutine(Color_Fade());
                    break;
                case TextSectionColor.Twinkling:
                    StartCoroutine(Color_Twinkling());
                    break;
                case TextSectionColor.Rainbow:
                    StartCoroutine(Color_Rainbow());
                    break;
            }
        }
    }

    public void Stop()
    {
        if (appearCoroutine != null) StopCoroutine(appearCoroutine);
        StartCoroutine(StopCoroutine());
    }

    IEnumerator StopCoroutine()
    {
        if (data.disappear != TextSectionDisappear.None)
        {
            switch (data.disappear)
            {
                case TextSectionDisappear.Fade:
                    yield return StartCoroutine(Disappear_Fade());
                    break;
                case TextSectionDisappear.Contraction:
                    yield return StartCoroutine(Disappear_Contraction());
                    break;
            }
        }
        
        StopAllCoroutines();
        BackdoorText.Instance.BackToPool(this);
    }

    IEnumerator Appear_Fade()
    {
        if (data.appearTime <= 0f)
        {
            appearCoroutine = null;
            yield break;
        }

        float alpha = 0.0f;
        Color newColor;
        while (alpha < 1.0f)
        {
            alpha += Time.deltaTime / data.appearTime;
            newColor = TMP.color;
            newColor.a = alpha;
            TMP.color = newColor;

            yield return null;
        }

        newColor = TMP.color;
        newColor.a = 1.0f;
        TMP.color = newColor;

        appearCoroutine = null;
    }

    IEnumerator Appear_Expansion()
    {
        if (data.appearTime <= 0f)
        {
            appearCoroutine = null;
            yield break;
        }

        Vector3 start = Vector3.zero;
        float t = 0.0f;
        while (t < data.appearTime)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, ogSize, t / data.appearTime);
            yield return null;
        }

        transform.localScale = ogSize;

        appearCoroutine = null;
    }

    IEnumerator Idle_Vibration()
    {
        WaitForSeconds waitTime = new WaitForSeconds(data.idleInterval);

        while (true)
        {
            Vector2 randomCircleOffset = Random.insideUnitCircle * data.idlePower;
            transform.localPosition = ogPos + (Vector3)randomCircleOffset;

            yield return waitTime;
        }
    }

    IEnumerator Idle_Wobble()
    {
        Vector3 ogPos = transform.localPosition;
        Vector3 targetPos = ogPos;
        Vector3 currentVelocity = Vector3.zero;
        Vector3 previousDirection = Vector3.zero;

        float accumulatedAngle = 0f;
        float distanceThreshold = 0.05f;

        void SetNewTarget()
        {
            Vector2 randomCircleOffset = Random.insideUnitCircle * data.idlePower;
            targetPos = ogPos + (Vector3)randomCircleOffset;
            accumulatedAngle = 0f;
        }

        SetNewTarget();

        while (true)
        {
            if (Vector3.Distance(transform.localPosition, targetPos) < distanceThreshold ||
                Mathf.Abs(accumulatedAngle) > 270f)
            {
                SetNewTarget();
            }

            Vector3 desiredDirection = (targetPos - transform.localPosition).normalized;

            Vector3 desiredVelocity = desiredDirection * data.idleInterval;

            float acceleration = data.idleInterval * 10f;

            currentVelocity = Vector3.MoveTowards(currentVelocity, desiredVelocity, acceleration * Time.deltaTime);

            transform.localPosition += currentVelocity * Time.deltaTime;

            if (currentVelocity.sqrMagnitude > 0.0001f)
            {
                Vector3 currentDirection = currentVelocity.normalized;

                if (previousDirection != Vector3.zero)
                {
                    float angleDifference = Vector3.SignedAngle(previousDirection, currentDirection, Vector3.forward);
                    accumulatedAngle += angleDifference;
                }

                previousDirection = currentDirection;
            }

            yield return null;
        }
    }

    IEnumerator Idle_Bobbing()
    {
        Vector3 ogPos = transform.localPosition;
        float time = 0f;

        float EaseInOutQuart(float x)
        {
            return x < 0.5f ? 8f * x * x * x * x : 1f - Mathf.Pow(-2f * x + 2f, 4f) / 2f;
        }

        while (true)
        {
            time += Time.deltaTime;

            float interval = Mathf.Max(data.idleInterval, 0.001f);
            float linearProgress = Mathf.PingPong(time / (interval / 2f) + 0.5f, 1f);
            float easedProgress = EaseInOutQuart(linearProgress);
            float currentY = (easedProgress - 0.5f) * data.idlePower;
            transform.localPosition = ogPos + new Vector3(0f, currentY, 0f);

            yield return null;
        }
    }

    IEnumerator Color_Fade()
    {
        float t = 0;
        Color color1 = data.mainColor;
        Color color2 = data.subColor;

        while (true)
        {
            while (t <= data.colorTransitionTime)
            {
                t += Time.deltaTime;
                TMP.color = Color.Lerp(color1, color2, t / data.colorTransitionTime);
                yield return null;
            }

            (color1, color2) = (color2, color1);
            t = 0.0f;
        }
    }

    IEnumerator Color_Twinkling()
    {
        WaitForSeconds wait = new WaitForSeconds(data.colorTransitionTime);
        while (true)
        {
            yield return wait;
            TMP.color = data.subColor;
            yield return wait;
            TMP.color = data.mainColor;
        }
    }

    IEnumerator Color_Rainbow()
    {
        float hue = 0f;
        Color nextColor;
        while (true)
        {
            hue += Time.deltaTime * data.colorTransitionTime;
            hue %= 1f;

            nextColor = Color.HSVToRGB(hue, 1f, 1f);
            TMP.color = nextColor;

            yield return null;
        }
    }

    IEnumerator Disappear_Fade()
    {
        if (data.disappearTime <= 0f)
        {
            yield break;
        }

        float alpha = 1.0f;
        Color newColor;
        while (alpha > 0f)
        {
            alpha -= Time.deltaTime / data.disappearTime;
            newColor = TMP.color;
            newColor.a = Mathf.Max(alpha, 0.0f);
            TMP.color = newColor;

            yield return null;
        }
    }

    IEnumerator Disappear_Contraction()
    {
        if (data.disappearTime <= 0f)
        {
            yield break;
        }

        Vector3 start = transform.localScale;
        Vector3 end = Vector3.zero;
        float t = 0.0f;
        while (t < data.disappearTime)
        {
            t += Time.deltaTime;
            transform.localScale = Vector3.Lerp(start, end, t / data.disappearTime);
            yield return null;
        }

        transform.localScale = ogSize;
    }
}
