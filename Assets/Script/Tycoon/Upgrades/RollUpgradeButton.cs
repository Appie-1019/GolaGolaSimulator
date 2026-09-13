using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(TycoonButtonTooltip))]
public class RollUpgradeButton : MonoBehaviour
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
        TycoonGameManager.Instance.MultiplierPoint(Random.Range(0,4));

        lv++;
        cost = GetCost();
        UpdateTooltip();
    }

    private int GetCost()
    {
        return (lv + 1) * 10;
    }

    private void UpdateTooltip()
    {
        string newTooltipText = $"<color=orange>[ROLL Lv{lv} >>> ROLL Lv{lv + 1}] ({cost}GJ)</color>\r\n현재 보유 중인 GJ에 <color=lightBlue>0, 1, 2, 3</color>중 하나를 랜덤하게 곱합니다.";
        tooltip.UpdateDescription(newTooltipText);
    }
}
