using UnityEngine;
using UnityEngine.UI;

public class SettingPanelToggle : MonoBehaviour
{
    public GameObject settingPanel;
    public ScrollRect settingScrollRect;

    bool isPanelOpen = false;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleSettingPanel();
        }
    }

    void ToggleSettingPanel()
    {
        isPanelOpen = !isPanelOpen;
        settingPanel.SetActive(isPanelOpen);
        if (isPanelOpen) settingScrollRect.verticalNormalizedPosition = 1f;
    }
}