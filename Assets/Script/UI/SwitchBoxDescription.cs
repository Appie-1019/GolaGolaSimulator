using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SwitchBoxDescription : MonoBehaviour
{
    [SerializeField] private SwitchBox switchBox;
    [SerializeField] private string[] description;

    private TextMeshProUGUI Text;
    private void Awake()
    {
        Text = GetComponent<TextMeshProUGUI>();
        if (switchBox == null || description == null)
        {
            Debug.LogError(gameObject.name + "은(는) 필요한 데이터가 없음");
            this.enabled = false;
            return;
        }
        switchBox.AddToggleListener(SetText);
    }

    public void SetText(SwitchBoxSection data)
    {
        if (data.SectionID >= description.Length) return;

        Text.text = description[data.SectionID];
    }
}
