using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackdoorTextPrinter : MonoBehaviour
{
    public static BackdoorTextPrinter Instance { get; private set; }

    [SerializeField] private BackdoorPrinterSection firstMeetingDialogue;
    [SerializeField] private BackdoorPrinterSection[] randomInDialogue;
    [SerializeField] private float rareChance = 1.0f;
    [SerializeField] private BackdoorPrinterSection[] rareInDialogue;
    [SerializeField] private BackdoorPrinterSection[] randomOutDialogue;
    [SerializeField] private AudioClip bgm;

    [HideInInspector] public List<BackDoorButon> sceneButtons;
    private System.Action onLastDialog;
    private System.Action onDialogEnd;
    private AudioInstance bgmInstance;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        if (DataManager.SaveData.Backdoor.backdoorDialogIndex == 0)
        {
            StartCoroutine(ShowText(firstMeetingDialogue, firstMeetingDialogue.firstWaitTime));
            onDialogEnd += StartFirstSequence;
        }
        else
        {
            StartFirstSequence();
        }
        DataManager.SaveData.Backdoor.backdoorDialogIndex++;
        DataManager.Save();

        bgmInstance = AudioManager.Instance.Play2DSound(bgm, SoundType.Game, 1, 1, true);
    }

    private void StartFirstSequence()
    {
        if (CheckProbability(rareChance))
        {
            int randomIndex = Random.Range(0, rareInDialogue.Length);
            StartCoroutine(ShowText(rareInDialogue[randomIndex], rareInDialogue[randomIndex].firstWaitTime));
        }
        else
        {
            int randomIndex = Random.Range(0, randomInDialogue.Length);
            StartCoroutine(ShowText(randomInDialogue[randomIndex], randomInDialogue[randomIndex].firstWaitTime));
        }

        onLastDialog += StartBackDoor;
    }

    private bool CheckProbability(float chance)
    {
        if (chance <= 0f) return false;
        if (chance >= 100f) return true;

        float randomValue = Random.Range(0f, 100f);
        return randomValue < chance;
    }

    private void StartBackDoor()
    {
        foreach (BackDoorButon b in sceneButtons)
        {
            b.gameObject.SetActive(true);
        }
    }

    IEnumerator ShowText(BackdoorPrinterSection data, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        WaitUntil waitForStop = new WaitUntil(WaitForStop);
        for (int i = 0; i < data.data.Length; i++)
        {
            BackdoorText.Instance.ShowText(data.data[i]);
            if (i == data.data.Length - 1)
            {
                onLastDialog?.Invoke();
                onLastDialog = null;
            }
            yield return waitForStop;
        }

        onDialogEnd?.Invoke();
        onDialogEnd = null;
    }

    private bool WaitForStop()
    {
        if (BackdoorText.Instance == null) return false;
        return !BackdoorText.Instance.IsShowingText;
    }

    public void GoToScene(string sceneName)
    {
        foreach (BackDoorButon b in sceneButtons)
        {
            b.gameObject.SetActive(false);
        }

        StopAllCoroutines();
        BackdoorText.Instance.StopAllTextImmediate();

        int randomIndex = Random.Range(0, randomOutDialogue.Length);
        StartCoroutine(ShowText(randomOutDialogue[randomIndex], randomOutDialogue[randomIndex].firstWaitTime));
        onDialogEnd += () =>
        {
            GameManager.TryLoadScene(sceneName);
        };
        bgmInstance.StopSound(1);
    }
}
