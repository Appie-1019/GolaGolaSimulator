using System.Collections;
using UnityEngine;

public class BackdoorTextPrinter : MonoBehaviour
{
    [System.Serializable]
    struct BackdoorTextPrinterSection
    {
        public float firstWaitTime;
        public BackdoorTextData[] data;
    }
    [SerializeField]
    private BackdoorTextPrinterSection[] sections;

    private void Start()
    {
        int currentIndex = DataManager.saveData.Backdoor.backdoorDialogIndex;
        StartCoroutine(ShowText(sections[currentIndex].data, sections[currentIndex].firstWaitTime));

        if (currentIndex + 1 < sections.Length)
        {
            DataManager.saveData.Backdoor.backdoorDialogIndex++;
            DataManager.Save();
        }
    }

    IEnumerator ShowText(BackdoorTextData[] data, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        WaitUntil waitForStop = new WaitUntil(WaitForStop);
        for (int i = 0; i < data.Length; i++)
        {
            BackdoorText.Instance.ShowText(data[i]);
            yield return waitForStop;
        }

        GameManager.TryLoadScene("Main");
    }

    private bool WaitForStop()
    {
        if (BackdoorText.Instance == null) return false;
        return !BackdoorText.Instance.IsShowingText;
    }
}
