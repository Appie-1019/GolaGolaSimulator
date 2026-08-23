using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroTypeWriter : MonoBehaviour
{
    [Serializable]
    private struct IntroDialogSection
    {
        public float typeDuration;
        [TextArea(3, 10)]
        public string text1;
        public float waitTime1;
        [TextArea(3, 10)]
        public string text2;
        public float waitTime2;
    }

    [SerializeField] private AudioClip soundEff;
    [SerializeField] private TextMeshProUGUI targetText1;
    [SerializeField] private TextMeshProUGUI targetText2;
    [SerializeField] private IntroDialogSection[] dialog;
    [SerializeField] private string nextSceneName;

    private void Start()
    {
        StartCoroutine(DialogAction());
    }

    private IEnumerator DialogAction()
    {
        for (int i = 0; i < dialog.Length; i++)
        {
            IntroDialogSection now = dialog[i];
            AudioManager.Instance?.Play2DSound(soundEff,SoundType.UI);
            yield return StartCoroutine(TypeTextCoroutine(now.text1, now.typeDuration, targetText1));
            yield return new WaitForSeconds(now.waitTime1);
            yield return StartCoroutine(TypeTextCoroutine(now.text2, now.typeDuration, targetText2));
            yield return new WaitForSeconds(now.waitTime2);

            StartCoroutine(EraseTextCoroutine(now.typeDuration, targetText1));
            yield return StartCoroutine(EraseTextCoroutine(now.typeDuration, targetText2));
            yield return null;
        }

        if (GameManager.IsSceneInBuildSettings(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator TypeTextCoroutine(string targetString, float totalDuration, TextMeshProUGUI targetText)
    {
        targetText.text = "";

        if (string.IsNullOrEmpty(targetString)) yield break;

        int totalLength = targetString.Length;
        float elapsedTime = 0f;

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / totalDuration);
            int charsToShow = Mathf.FloorToInt(progress * totalLength);

            targetText.text = targetString.Substring(0, charsToShow);
            yield return null;
        }

        targetText.text = targetString;
    }

    private IEnumerator EraseTextCoroutine(float totalDuration, TextMeshProUGUI targetText)
    {
        string currentString = targetText.text;

        if (string.IsNullOrEmpty(currentString)) yield break;

        int totalLength = currentString.Length;
        float elapsedTime = 0f;

        while (elapsedTime < totalDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / totalDuration);
            int charsToKeep = totalLength - Mathf.FloorToInt(progress * totalLength);
            charsToKeep = Mathf.Max(0, charsToKeep);

            targetText.text = currentString.Substring(0, charsToKeep);
            yield return null;
        }

        targetText.text = "";
    }
}
