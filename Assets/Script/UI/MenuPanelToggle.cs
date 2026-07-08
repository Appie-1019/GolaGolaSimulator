using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MenuPanelToggle : MonoBehaviour
{
    [Header("Panel")]
    public CanvasGroup settingPanel;
    public ScrollRect settingScrollRect;
    public float fadeDuration = 0.2f;
    public bool DisableOnStart = true;
    [Header("Sliders")]
    public Slider[] allSliders;

    public static bool isPanelOpen = false;
    private float touchHoldTimer = 0f;

    private void Start()
    {
        if (DisableOnStart)
        {
            isPanelOpen = true;
            ToggleMenuPanel(true);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            ToggleMenuPanel();
        }

        if (DataManager.isMobile && CanToggleMenuPanelOnMoblie())
        {
            ToggleMenuPanel();
        }
    }

    private bool CanToggleMenuPanelOnMoblie()
    {
        if (Input.touchCount == 2)
        {
            if (touchHoldTimer < 0f) return false;

            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchZero.phase != TouchPhase.Ended && touchZero.phase != TouchPhase.Canceled &&
                touchOne.phase != TouchPhase.Ended && touchOne.phase != TouchPhase.Canceled)
            {
                touchHoldTimer += Time.deltaTime;

                if (touchHoldTimer >= 0.6887f)
                {
                    touchHoldTimer = float.MinValue;
                    return true;
                }
            }
        }
        else
        {
            touchHoldTimer = 0f;
        }

        return false;
    }

    void ToggleMenuPanel(bool instant = false)
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
                GameObject currentSelected = EventSystem.current.currentSelectedGameObject;

                if (currentSelected != null)
                {
                    if (currentSelected.TryGetComponent<InputFieldValueLimiter>(out InputFieldValueLimiter floatLimiter))
                    {
                        floatLimiter.CheckValue();
                    }
                    else if (currentSelected.TryGetComponent<InputFieldHexLimiter>(out InputFieldHexLimiter hexLimiter))
                    {
                        hexLimiter.CheckValue();
                    }
                }

                if (allSliders != null && allSliders.Length != 0)
                {
                    foreach (var sliders in allSliders)
                    {
                        if (sliders != null) sliders.ForceReleaseDrag();
                    }
                }

                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }
}