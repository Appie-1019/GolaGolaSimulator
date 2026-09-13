using TMPro;
using UnityEngine;

public class TycoonUpgradeManager : MonoBehaviour
{
    public static TycoonUpgradeManager Instance { get; private set; }
    public TextMeshProUGUI toolTipText;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void ShowToolTip(string text)
    {
        toolTipText.text = text;
    }

    public void HideToolTip()
    {
        toolTipText.text = string.Empty;
    }
}
