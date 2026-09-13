using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(TycoonButtonTooltip))]
public class FreeMoneyUpgradeButton : MonoBehaviour
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
        TycoonGameManager.Instance.AddPoints(300 * lv + 500); // 300*<Lv>+500

        lv++;
        cost = GetCost();
        UpdateTooltip();
    }

    private int GetCost()   // 100*(1.3^(<Lv> + 1))
    {
        return 100 * (int)Mathf.Pow(1.3f, lv + 1);
    }

    private void UpdateTooltip()
    {
        string newTooltipText = $"<color=orange>[꽁돈 Lv{lv} >>> 꽁돈 Lv{lv + 1}] ({cost}GJ)</color>\r\n레벨에 비례하여 GJ을 지급합니다!";
        tooltip.UpdateDescription(newTooltipText);
    }
}
