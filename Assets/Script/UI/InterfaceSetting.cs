using System.Collections;
using UnityEngine;

public class InterfaceSetting : MonoBehaviour
{
    public ToggleSwitch toastToggleSwitch;
    public string[] toastEnableMessages;
    public string[] toastDisableMessages;

    void Start()
    {
        toastToggleSwitch.InitEnable(DataManager.SaveData.UI.ToastMessageAllow, true);
        StartCoroutine(StartAct());
    }

    IEnumerator StartAct()
    {
        yield return null;
        toastToggleSwitch.AddToggleListener(ToastMessageEnable);
    }

    void ToastMessageEnable(bool enable)
    {
        if (enable)
        {
            DataManager.SaveData.UI.ToastMessageAllow = enable;
            ToastUIManager.Instance?.AddToast(GetRandomMessage(toastEnableMessages), Color.yellow);
        }
        else
        {
            ToastUIManager.Instance.AddToast(GetRandomMessage(toastDisableMessages), Color.yellow);
            DataManager.SaveData.UI.ToastMessageAllow = enable;
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
