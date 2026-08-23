using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CreditPrinter : MonoBehaviour
{
    [System.Serializable]
    private struct CreditTextGroup
    {
        public string title;
        public float titleSize;

        public string[] detail;
        public float detailSize;
    }

    [Header("TextObject")]
    public Canvas canvas;
    public CreditText textSection;
    [Header("Text")]
    [SerializeField] private CreditTextGroup[] texts;
    [Header("NextScene")]
    public string nextScene;

    public static List<CreditText> allText;

    private float currentDeltaTimeFactor = 1f;
    private float acceleration = 50f;
    bool stoped = false;

    void Start()
    {
        stoped = false;
        allText = new List<CreditText>();
        StartCoroutine(PrintCredit());
    }

    private void Update()
    {
        if (stoped) return;
        if (Pointer.current == null) return;

        if (Pointer.current.press.isPressed)
        {
            TimeManager.CustomDeltaTimeFactor = currentDeltaTimeFactor;
            currentDeltaTimeFactor += acceleration * Time.deltaTime;
        }
        else if (Pointer.current.press.wasReleasedThisFrame)
        {
            TimeManager.CustomDeltaTimeFactor = currentDeltaTimeFactor = 1;
        }
    }

    IEnumerator PrintCredit()
    {
        AddText("GolaGolaSimulator", Color.gold, 100f);
        yield return WaitForCustomTime(1);
        AddText("V1.5", Color.gold, 80f);
        yield return WaitForCustomTime(3);

        for (int i = 0; i < texts.Length; i++)
        {
            yield return StartCoroutine(PrintTextGroup(texts[i]));
            yield return WaitForCustomTime(2);
        }

        yield return new WaitUntil(() => allText.Count == 0);
        stoped = true;
        TimeManager.CustomDeltaTimeFactor = 1;

        yield return new WaitForSeconds(1.5f);

        if (GameManager.IsSceneInBuildSettings(nextScene))
        {
            SceneManager.LoadScene(nextScene);
        }
        else
        {
            Debug.LogWarning("이동할 씬 이름이 올바르지 않음");
        }
    }

    IEnumerator PrintTextGroup(CreditTextGroup group)
    {
        AddText(group.title, group.titleSize);
        yield return WaitForCustomTime(1);
        if (group.detail != null && group.detail.Length > 0)
        {
            for (int i = 0; i < group.detail.Length; i++)
            {
                AddText(group.detail[i], group.detailSize);
                yield return WaitForCustomTime(1);
            }
        }
    }

    IEnumerator WaitForCustomTime(float waitTime)
    {
        float t = 0;
        while (waitTime >= t)
        {
            t += TimeManager.CustomDeltaTime;
            yield return null;
        }
    }

    void AddText(string text, float size = 50f)
    {
        AddText(text, Color.white, size);
    }

    void AddText(string text, Color color, float size)
    {
        CreditText newText = Instantiate(textSection, canvas.transform);
        newText.SetText(text);
        newText.SetFontSize(size);
        newText.SetColor(color);
        newText.canvas = canvas;
    }
}
