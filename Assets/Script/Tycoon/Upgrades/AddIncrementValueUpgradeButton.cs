using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(TycoonButtonTooltip))]
public class AddIncrementValueUpgradeButton : MonoBehaviour
{
    private Button button;
    private TycoonButtonTooltip tooltip;
    private int lv;
    private int cost;

    private void Awake()
    {
        tooltip = GetComponent<TycoonButtonTooltip>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        lv = 0;
        cost = GetCost();
        UpdateTooltip();
    }

    private void OnClick()
    {
        if (TycoonGameManager.Instance.totalPoints < cost) return;
        TycoonGameManager.Instance.AddPoints(-cost);
        TycoonGameManager.Instance.incrementValue *= 2;

        lv++;
        cost = GetCost();
        UpdateTooltip();
    }

    private int GetCost()
    {
        return (int)Mathf.Sqrt(lv + 1) * 50;
    }

    private void UpdateTooltip()
    {
        string newTooltipText = $"<color=orange>[획득량 증가 Lv{lv} >>> 획득량 증가 Lv{lv + 1}] ({cost}GJ)</color>\r\n한 번 GolaGola할 때마다 얻는 GJ가 <color=lightBlue>2배 증가</color>합니다.";
        tooltip.UpdateDescription(newTooltipText);
    }
}
