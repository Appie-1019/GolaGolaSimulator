using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(TycoonButtonTooltip))]
public class MoneyGlichUpgradeButton : MonoBehaviour
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

        if (lv < 2) TycoonGameManager.Instance.MultiplierPoint(2);
        else TycoonGameManager.Instance.SetPoint(0);

        lv++;
        cost = GetCost();
        UpdateTooltip();
    }

    private int GetCost()
    {
        return 150 * (lv + 1);
    }

    private void UpdateTooltip()
    {
        string newTooltipPrefix = $"<color=orange>[돈 복사 버그 Lv{lv} >>> 돈 복사 버그 Lv{lv + 1}] ({cost}GJ)</color>\r\n";
        string newTooltipText;

        if (lv < 2) newTooltipText = "보유 GJ을 두 배 증가시킵니다!!";
        else if (lv == 2) newTooltipText = "보유 GJ을 두 배 증가시킵니다??";
        else newTooltipText = "보유 GJ을 모조리 없앨테다.";

        tooltip.UpdateDescription(newTooltipPrefix + newTooltipText);
    }
}
