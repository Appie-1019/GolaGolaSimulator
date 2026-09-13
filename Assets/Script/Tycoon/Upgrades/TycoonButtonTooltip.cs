using UnityEngine;
using UnityEngine.EventSystems;

public class TycoonButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField]
    [TextArea(3, 10)]
    private string description;

    private bool isPointerHere;

    public void OnPointerEnter(PointerEventData eventData)
    {
        isPointerHere = true;
        if (TycoonUpgradeManager.Instance == null) return;
        TycoonUpgradeManager.Instance.ShowToolTip(description);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        isPointerHere = false;
        if (TycoonUpgradeManager.Instance == null) return;
        TycoonUpgradeManager.Instance.HideToolTip();
    }

    public void UpdateDescription(string text)
    {
        description = text;
        if (isPointerHere)
        {
            if (TycoonUpgradeManager.Instance == null) return;
            TycoonUpgradeManager.Instance.ShowToolTip(description);
        }
    }
}
