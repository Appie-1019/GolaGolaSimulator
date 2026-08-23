using DG.Tweening;
using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ToastUI : MonoBehaviour
{
    CanvasGroup cg;
    RectTransform rect;
    TextMeshProUGUI message;
    Sequence seq;
    Coroutine destroyCoroutine;
    Coroutine cutTextCoroutine;
    Tween fontSizeTween;
    ToastUIData data;
    int currentAccumulationCount = 0;
    bool isDestroying;
    Vector2 currentAnchoredPos;

    private void Awake()
    {
        cg = GetComponent<CanvasGroup>();
        rect = GetComponent<RectTransform>();
        message = GetComponentInChildren<TextMeshProUGUI>();

        cg.alpha = 0f;
        cg.interactable = cg.blocksRaycasts = isDestroying = false;
    }

    /// <summary> 토스트 메시지 표시 시작 </summary>
    /// <param name="messageText">표시할 글자</param>
    public void Show(string messageText)
    {
        if (isDestroying) return;
        if (string.IsNullOrWhiteSpace(messageText))
        {
            Destroy(gameObject);
            return;
        }
        StopAllCoroutines();
        KillAllTweens();

        currentAccumulationCount = 0;
        rect.anchoredPosition = data.StartAnchoredPosition;

        seq?.Kill();
        message.text = messageText;
        cg.alpha = 0f;

        seq = DOTween.Sequence();
        seq.Append(
            cg.DOFade(1f, data.MoveDuration)
        );
        seq.Join(
            rect.DOAnchorPos(data.EndAnchoredPosition, data.MoveDuration)
                .SetEase(Ease.OutQuad)
        );
        currentAnchoredPos = data.EndAnchoredPosition;
        destroyCoroutine = StartCoroutine(DestroyTimer());
    }

    /// <summary> 파괴 시간 대기 후 <see cref="DestroyImmediately"/> 호출 </summary>
    IEnumerator DestroyTimer()
    {
        yield return new WaitForSeconds(data.ShowTime);
        DestroyImmediately();
    }

    /// <summary> 토스트 메시지를 애니메이션과 함께 파괴 </summary>
    void DestroyImmediately()
    {
        if (destroyCoroutine != null) StopCoroutine(destroyCoroutine);
        if (cutTextCoroutine != null) StopCoroutine(cutTextCoroutine);
        if (currentAccumulationCount > 1) CutTextImmediately(message, data.DisplacementCharactersNumber);

        KillAllTweens();
        isDestroying = true;
        seq = DOTween.Sequence().SetLink(gameObject);
        seq.SetLink(gameObject);
        seq.Append(
            cg.DOFade(0f, data.MoveDuration)
            .SetLink(gameObject)
        );
        seq.Join(
            rect.DOAnchorPos(currentAnchoredPos + data.DestroyOffset, data.MoveDuration)
                .SetEase(Ease.OutQuad)
                .SetLink(gameObject)
        );
        seq.OnComplete(() => { Destroy(gameObject); });
    }

    /// <summary> 토스트 메시지의 쌓임 정도 추가 </summary>
    public void Stack()
    {
        if (isDestroying) return;
        currentAccumulationCount++;
        if (currentAccumulationCount >= data.MaxAccumulation)
        {
            DestroyImmediately();
        }
        else if (currentAccumulationCount == 1)
        {
            KillAllTweens();
            cg.alpha = 1f;
            currentAnchoredPos += data.FirstDisplacementOffset;
            rect.DOAnchorPos(currentAnchoredPos, data.MoveDuration).SetEase(Ease.OutQuad);
            rect.DOSizeDelta(data.FirstDisplacementSizeDelta, data.MoveDuration).SetEase(Ease.OutQuad);
            fontSizeTween = DOTween.To(() => message.fontSize, size => message.fontSize = size, data.DisplacementTextFontSize, data.MoveDuration);
            cutTextCoroutine = StartCoroutine(CutText(message, data.DisplacementCharactersNumber, data.MoveDuration));
        }
        else
        {
            KillAllTweens();
            rect.sizeDelta = data.FirstDisplacementSizeDelta;
            message.fontSize = data.DisplacementTextFontSize;
            currentAnchoredPos += data.DisplacementOffset;
            rect.DOAnchorPos(currentAnchoredPos, data.MoveDuration).SetEase(Ease.OutQuad);
        }
    }

    /// <summary> 이 개체의 <see cref="data"/> 를 <paramref name="data"/>로 지정 </summary>
    /// <param name="data">넣을 데이터</param>
    public void SetData(ToastUIData data) => this.data = data;

    /// <summary> <paramref name="duration"/>동안 <paramref name="tmp"/>의 글자 수를 <paramref name="maxCharactersNumber"/>로 제한하고 <paramref name="lastMessege"/> 추가 </summary>
    /// <param name="tmp"> 대상 <see cref="TextMeshProUGUI"/> </param>
    /// <param name="maxCharactersNumber"> 제한될 최대 글자 수 </param>
    /// <param name="duration"> 동작 기간 </param>
    /// <param name="lastMessege"> <paramref name="tmp"/>가 제한되었을 경우, 맨 뒤에 추가할 메시지</param>
    IEnumerator CutText(TextMeshProUGUI tmp, int maxCharactersNumber, float duration, string lastMessege = "...")
    {
        if (tmp == null || string.IsNullOrEmpty(tmp.text)) yield break;

        if (tmp.text.Length < maxCharactersNumber) yield break;

        string originalText = tmp.text;
        int originalLength = originalText.Length;

        int totalSteps = (originalLength - maxCharactersNumber) + lastMessege.Length;
        float interval = duration / totalSteps;

        for (int i = originalLength - 1; i >= maxCharactersNumber; i--)
        {
            yield return new WaitForSeconds(interval);
            tmp.text = originalText.Substring(0, i);
        }

        yield return new WaitForSeconds(interval);

        string baseText = originalText.Substring(0, maxCharactersNumber);
        for (int i = 1; i <= lastMessege.Length; i++)
        {
            tmp.text = baseText + lastMessege.Substring(0, i);

            if (i < lastMessege.Length) yield return new WaitForSeconds(interval);
        }
    }

    /// <summary> 즉시 <paramref name="tmp"/>의 글자 수를 <paramref name="maxCharactersNumber"/>로 제한하고 <paramref name="lastMessege"/> 추가 </summary>
    /// <param name="tmp"> 대상 <see cref="TextMeshProUGUI"/> </param>
    /// <param name="maxCharactersNumber"> 제한될 최대 글자 수 </param>
    /// <param name="lastMessege"> <paramref name="tmp"/>가 제한되었을 경우, 맨 뒤에 추가할 메시지</param>
    void CutTextImmediately(TextMeshProUGUI tmp, int maxCharactersNumber, string lastMessege = "...")
    {
        if (tmp == null || string.IsNullOrEmpty(tmp.text)) return;

        if (tmp.text.Length < maxCharactersNumber) return;

        string baseText = tmp.text.Substring(0, maxCharactersNumber);
        tmp.text = baseText + lastMessege;
    }

    /// <summary> 작동 중인 모든 <see cref="Tween"/>과 <see cref="Sequence"/> 중단 </summary>
    private void KillAllTweens()
    {
        seq?.Kill();
        fontSizeTween?.Kill();
        if (rect != null) rect.DOKill();
    }

    private void OnDestroy()
    {
        KillAllTweens();
    }
}