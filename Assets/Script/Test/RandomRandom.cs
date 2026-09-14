using System.Collections;
using TMPro;
using UnityEngine;
using static UnityEngine.AdaptivePerformance.Provider.AdaptivePerformanceSubsystemDescriptor;

public class RandomRandom : MonoBehaviour
{
    [Header("Set")]
    public RandomBox boxPrefab;
    public int boxCount = 10;
    public float boxInterval = 0.15f;
    [Header("Act")]
    public bool useAwesomeRoll = true;
    public int tryCount;
    public float scaleValue;
    public float waitTime;
    [Header("UI")]
    public TextMeshProUGUI info;

    private RandomBox[] hambur;

    private void Start()
    {
        info.text = string.Empty;
        SetBox();
        if (useAwesomeRoll) StartCoroutine(Act_New());
        else StartCoroutine(Act());
    }

    public void SetBox()
    {
        hambur = new RandomBox[boxCount];
        Vector2 startPos = Vector2.zero;
        startPos.x = -(boxCount * boxInterval) / 2;

        for (int i = 0; i < boxCount; i++)
        {
            hambur[i] = Instantiate(boxPrefab, startPos, Quaternion.identity);
            hambur[i].name = $"Line_{i:D3}";
            startPos.x += boxInterval;
        }
    }

    IEnumerator Act()
    {
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        int randomIndex = 0;
        int prevIndex = 0;
        for (int i = 0; i < tryCount; i++)
        {
            if (waitTime > 0) yield return wait;
            randomIndex = Random.Range(0, hambur.Length);
            hambur[prevIndex].ResetColor();
            hambur[randomIndex].AddScaleY(scaleValue);
            prevIndex = randomIndex;
            info.text = $"In Progress ({i + 1}/{tryCount})\n\rSelected Line : <color=orange>{hambur[randomIndex].name}";
        }
        hambur[prevIndex].ResetColor();

        StartCoroutine(GetDiff());
    }

    IEnumerator Act_New()
    {
        WaitForSeconds wait = new WaitForSeconds(waitTime);
        int randomIndex1 = 0;
        int randomIndex2 = 0;
        int prevIndex1 = 0;
        int prevIndex2 = 0;
        for (int i = 0; i < tryCount; i++)
        {
            if (waitTime > 0) yield return wait;

            hambur[prevIndex1].ResetColor();
            hambur[prevIndex2].ResetColor();

            randomIndex1 = Random.Range(0, hambur.Length);
            randomIndex2 = Random.Range(0, hambur.Length);
            while (randomIndex1 == randomIndex2) randomIndex2 = Random.Range(0, hambur.Length);

            hambur[randomIndex1].SetColorSelected();
            hambur[randomIndex2].SetColorSelected();
            prevIndex1 = randomIndex1;
            prevIndex2 = randomIndex2;

            info.text = $"In Progress ({i+1}/{tryCount})\n\rSelected Line : <color=orange>{hambur[randomIndex1].name}({hambur[randomIndex1].selectedCount})</color> AND <color=orange>{hambur[randomIndex2].name}({hambur[randomIndex2].selectedCount})</color>\n\n\r";

            if (waitTime > 0) yield return wait;

            if (hambur[randomIndex1].selectedCount < hambur[randomIndex2].selectedCount)
            {
                hambur[randomIndex1].AddScaleY(scaleValue);

                info.text += $"{hambur[randomIndex1].name} was Selected";
            }
            else
            {
                hambur[randomIndex2].AddScaleY(scaleValue);

                info.text += $"{hambur[randomIndex2].name} was Selected";
            }
        }

        hambur[prevIndex1].ResetColor();
        hambur[prevIndex2].ResetColor();

        StartCoroutine(GetDiff());
    }

    IEnumerator GetDiff()
    {
        WaitForSeconds wait = new WaitForSeconds(0.05f);
        int min = int.MaxValue;
        int max = 0;

        for (int i = 0; i < hambur.Length; i++)
        {
            info.text = $"Testing in progress ({i + 1}/{hambur.Length})\n\n\r";

            if (hambur[i].selectedCount > max) max = hambur[i].selectedCount;
            if (hambur[i].selectedCount < min) min = hambur[i].selectedCount;

            hambur[i].SetColorSelected();
            if (i > 0) hambur[i - 1].ResetColor();

            info.text += $"Min : {min}\n\rMax : {max}";
            yield return wait;
        }
        hambur[hambur.Length - 1].ResetColor();

        info.text = $"Min : {min}\n\rMax : {max}\n\n\rDiff : {max - min}";
    }

}
