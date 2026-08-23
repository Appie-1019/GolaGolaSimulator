using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class SettingPanelToggle : MonoBehaviour
{
    public CanvasGroup settingPanel;
    public ScrollRect settingScrollRect;
    public float fadeDuration = 0.2f;

    public bool DisableOnStart = true;

    bool isPanelOpen = false;

    private void Start()
    {
        if (DisableOnStart)
        {
            isPanelOpen = true;
            ToggleSettingPanel(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleSettingPanel();
        }
    }

    void ToggleSettingPanel(bool instant = false)
    {
        isPanelOpen = !isPanelOpen;

        int alpha = isPanelOpen ? 1 : 0;
        if (instant) settingPanel.alpha = alpha;
        else settingPanel.DOFade(alpha, fadeDuration);

        settingPanel.interactable = isPanelOpen;
        settingPanel.blocksRaycasts = isPanelOpen;

        if (isPanelOpen) settingScrollRect.verticalNormalizedPosition = 1f;
    }
}