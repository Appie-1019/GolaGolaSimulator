using System.Collections;
using UnityEngine;

public class InterfaceSetting : MonoBehaviour
{
    public ToggleSwitch toastToggleSwitch;
    public string[] toastEnableMesseges;
    public string[] toastDisableMesseges;

    void Start()
    {
        toastToggleSwitch.InitEnable(DataManager.saveData.ToastMessegeAllow, true);
        StartCoroutine(StartAct());
    }

    IEnumerator StartAct()
    {
        yield return null;
        toastToggleSwitch.AddToggleListener(ToastMessegeEnable);
    }

    void ToastMessegeEnable(bool enable)
    {
        if (enable)
        {
            DataManager.saveData.ToastMessegeAllow = enable;
            ToastUIManager.Instance?.AddToast(GetRandomMessage(toastEnableMesseges), Color.yellow);
        }
        else
        {
            ToastUIManager.Instance.AddToast(GetRandomMessage(toastDisableMesseges), Color.yellow);
            DataManager.saveData.ToastMessegeAllow = enable;
        }

        DataManager.Save();
    }

    public string GetRandomMessage(string[] messages)
    {
        if (messages == null || messages.Length == 0)
        {
            return string.Empty;
        }

        int randomIndex = Random.Range(0, messages.Length);
        return messages[randomIndex];
    }
}
