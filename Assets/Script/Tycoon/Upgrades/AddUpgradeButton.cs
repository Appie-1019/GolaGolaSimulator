using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(TycoonButtonTooltip))]
public class AddUpgradeButton : MonoBehaviour
{
    public int cost = 50;
    [TextArea(3, 10)]
    public string[] tooltipTexts;
    public GameObject addIncrementValueUpgrade;
    public GameObject rollUpgrade;
    public GameObject freeMoneyUpgrade;
    public GameObject moneyGlichUpgrade;
    public GameObject grassUpgrade;

    private int lv = 0;
    private Button button;
    private TycoonButtonTooltip tooltip;

    private void Awake()
    {
        tooltip = GetComponent<TycoonButtonTooltip>();
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        tooltip.UpdateDescription(tooltipTexts[0]);
    }

    private void OnClick()
    {
        if (TycoonGameManager.Instance.totalPoints < cost) return;

        TycoonGameManager.Instance.AddPoints(-cost);
        lv++;

        if (tooltipTexts.Length > lv) tooltip.UpdateDescription(tooltipTexts[lv]);

        switch (lv)
        {
            case 1:
                rollUpgrade.SetActive(true); break;
            case 2:
                freeMoneyUpgrade.SetActive(true); break;
            case 3:
                addIncrementValueUpgrade.SetActive(true); break;
            case 4:
                moneyGlichUpgrade.SetActive(true); break;
            case 5:
                grassUpgrade.SetActive(true);
                break;
        }

        if (lv >= 5)
        {
            string newTooltipText = $"<color=orange>[업그레이드 추가 Lv{lv} >>> 업그레이드 추가 Lv{lv + 1}] (50GJ)</color>\r\n이후 아무 업그레이드 콘텐츠가 없기 때문에 돈을 낭비할 수 있습니다.";
            tooltip.UpdateDescription(newTooltipText);
        }
    }
}
