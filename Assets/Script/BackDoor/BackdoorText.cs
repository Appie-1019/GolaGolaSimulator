using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

[System.Serializable]
public struct BackdoorTextData
{
    public BackdoorTextDataSection[] textSections;
    public float textInterval;
    public float textSize;
    public float textSpacing;
    public float showDuration;
}

[System.Serializable]
public struct BackdoorTextDataSection
{
    [Header("Text")]
    public string text;
    public Color mainColor;
    public Color subColor;

    [Header("Time")]
    public float colorTransitionTime;
    public float appearTime;
    public float disappearTime;

    [Header("Operation")]
    public float idlePower;
    public float idleInterval;
    public TextSectionAppear appear;
    public TextSectionIdle idle;
    public TextSectionDisappear disappear;
    public TextSectionColor color;
    [Header("Sound")]
    public AudioClip[] typeSounds;
}

public class BackdoorText : MonoBehaviour
{
    public static BackdoorText Instance { get; private set; }
    public bool IsShowingText
    {
        get
        {
            return showTextCoroutine != null;
        }
    }

    [Header("TextInstance")]
    public BackdoorTextSection textInstancePrefab;
    public int initialPoolSize = 20;
    private Queue<BackdoorTextSection> textPool = new Queue<BackdoorTextSection>();
    private List<BackdoorTextSection> activeText = new List<BackdoorTextSection>();
    private int currentPoolSize = 0;
    private Coroutine showTextCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        currentPoolSize = 0;
        for (int i = 0; i < initialPoolSize; i++)
        {
            BackdoorTextSection newInstance = Instantiate(textInstancePrefab, transform);
            newInstance.gameObject.SetActive(false);
            currentPoolSize++;
            newInstance.name = $"TextInstance[{currentPoolSize}]";
            textPool.Enqueue(newInstance);
        }
    }

    public void ShowText(BackdoorTextData data)
    {
        if (showTextCoroutine != null)
        {
            for (int i = 0; i < activeText.Count; i++)
            {
                activeText[i].Stop();
            }
            StopCoroutine(showTextCoroutine);
        }

        showTextCoroutine = StartCoroutine(ShowTextCoroutine(data));
    }

    public void BackToPool(BackdoorTextSection section)
    {
        textPool.Enqueue(section);
        activeText.Remove(section);
        section.gameObject.SetActive(false);
    }

    IEnumerator ShowTextCoroutine(BackdoorTextData data)
    {
        int textLength = GetTotalTextLength(data);
        char[] texts = GetAllCharacters(data);
        float[] pos = GetCharacterPositions(textLength, data.textSpacing);
        BackdoorTextDataSection[] sections = GetAllSections(data);

        WaitForSeconds waitInterval = data.textInterval > 0.0f ? new WaitForSeconds(data.textInterval) : null;
        for (int i = 0; i < textLength; i++)
        {
            if (texts[i] == '\0') continue;

            BackdoorTextSection newInstance;
            if (textPool.Count > 0)
            {
                newInstance = textPool.Dequeue();
            }
            else
            {
                newInstance = Instantiate(textInstancePrefab, transform);
                currentPoolSize++;
                newInstance.name = $"TextInstance[{currentPoolSize}]";
                Debug.Log($"백도어 텍스트 폴링 갯수 증가됨 : 현재 [{currentPoolSize}]개"); // 자주 호출된다면 initialPoolSize 값을 늘리는 것을 고려할 것.
            }

            Vector3 textPos = Vector3.zero;
            textPos.x = pos[i];

            newInstance.transform.position = textPos;
            newInstance.TMP.text = texts[i].ToString();
            newInstance.TMP.fontSize = data.textSize;
            newInstance.gameObject.SetActive(true);
            activeText.Add(newInstance);
            BackdoorTextDataSection currentSection = sections[i];
            newInstance.Init(currentSection);

            AudioManager.Instance?.PlayRandom2DSound(currentSection.typeSounds, SoundType.Game);

            if (waitInterval != null) yield return waitInterval;
        }

        yield return new WaitForSeconds(data.showDuration);

        List<BackdoorTextSection> sectionsToStop = new List<BackdoorTextSection>(activeText);
        for (int i = 0; i < sectionsToStop.Count; i++)
        {
            sectionsToStop[i].Stop();
        }

        yield return new WaitUntil(() => activeText.Count == 0);

        showTextCoroutine = null;
    }

    public int GetTotalTextLength(BackdoorTextData textData)
    {
        if (textData.textSections == null) return 0;

        int totalLength = 0;

        foreach (BackdoorTextDataSection section in textData.textSections)
        {
            if (!string.IsNullOrEmpty(section.text))
            {
                totalLength += section.text.Length;
            }
        }

        return totalLength;
    }

    public char GetCharacterAtIndex(BackdoorTextData textData, int index)
    {
        if (textData.textSections == null || index < 0)
        {
            return '\0';
        }

        int remainingIndex = index;

        for (int i = 0; i < textData.textSections.Length; i++)
        {
            string currentText = textData.textSections[i].text;

            if (string.IsNullOrEmpty(currentText))
            {
                continue;
            }

            if (remainingIndex < currentText.Length)
            {
                return currentText[remainingIndex];
            }

            remainingIndex -= currentText.Length;
        }

        return '\0';
    }

    public char[] GetAllCharacters(BackdoorTextData textData)
    {
        int totalLength = GetTotalTextLength(textData);

        if (totalLength == 0)
        {
            return new char[0];
        }

        char[] resultArray = new char[totalLength];
        int currentIndex = 0;

        for (int i = 0; i < textData.textSections.Length; i++)
        {
            string currentText = textData.textSections[i].text;

            if (string.IsNullOrEmpty(currentText))
            {
                continue;
            }

            for (int j = 0; j < currentText.Length; j++)
            {
                char c = currentText[j];
                resultArray[currentIndex] = char.IsWhiteSpace(c) ? '\0' : c;
                currentIndex++;
            }
        }

        return resultArray;
    }

    public float[] GetCharacterPositions(int count, float spacing)
    {
        if (count <= 0) return new float[0];

        float[] positions = new float[count];
        float startPos = GetStartPosition(count, spacing);

        for (int i = 0; i < count; i++)
        {
            positions[i] = startPos + (i * spacing);
        }

        return positions;
    }

    public BackdoorTextDataSection[] GetAllSections(BackdoorTextData textData)
    {
        int totalLength = GetTotalTextLength(textData);

        if (totalLength == 0)
        {
            return new BackdoorTextDataSection[0];
        }

        BackdoorTextDataSection[] resultArray = new BackdoorTextDataSection[totalLength];
        int currentIndex = 0;

        for (int i = 0; i < textData.textSections.Length; i++)
        {
            string currentText = textData.textSections[i].text;

            if (string.IsNullOrEmpty(currentText))
            {
                continue;
            }

            for (int j = 0; j < currentText.Length; j++)
            {
                resultArray[currentIndex] = textData.textSections[i];
                currentIndex++;
            }
        }

        return resultArray;
    }

    public float GetStartPosition(int count, float spacing)
    {
        if (count <= 0) return 0f;
        return -((count - 1) * spacing) / 2f;
    }
}

public enum TextSectionAppear
{
    None, Fade, Expansion
}

public enum TextSectionIdle
{
    None, Vibration, Wobble, Bobbing
}

public enum TextSectionDisappear
{
    None, Fade, Contraction
}

public enum TextSectionColor
{
    None, Fade, Twinkling, Rainbow
}