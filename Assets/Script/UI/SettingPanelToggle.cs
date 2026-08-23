using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SettingPanelToggle : MonoBehaviour
{
    public CanvasGroup settingPanel;
    public ScrollRect settingScrollRect;
    public float fadeDuration = 0.2f;

    public bool DisableOnStart = true;

    private bool isPanelOpen = false;
    private float lastClickTime;

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
        if (DataManager.isMobile)
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject()) return;

            if (Pointer.current.press.wasPressedThisFrame)
            {
                if (Time.time - lastClickTime < 0.2f)
                {
                    ToggleSettingPanel();
                    lastClickTime = 0;
                }
                else
                {
                    lastClickTime = Time.time;
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                ToggleSettingPanel();
            }
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
        else
        {
            if (EventSystem.current != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}