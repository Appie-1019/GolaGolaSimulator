using System.Collections;
using TMPro;
using UnityEngine;

public class UselessSwitch : MonoBehaviour
{
    public ToggleSwitch toggleSwitch;
    public BackgroundPreview backgroundPreview;
    int toggleCount = 0;

    private void Start()
    {
        if (toggleSwitch == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
        toggleSwitch.AddToggleListener(OnToggleChanged);
    }

    private void OnToggleChanged(bool isOn)
    {
        toggleCount++;
        if (toggleCount == 30)
        {
            TextMeshProUGUI tmp = GetComponent<TextMeshProUGUI>();
            tmp.text = "GolaGola?";
            StartCoroutine(RainbowTMP(tmp));
            backgroundPreview.DoARainbow();
        }
    }

    IEnumerator RainbowTMP(TextMeshProUGUI tmp)
    {
        while (true)
        {
            tmp.color = backgroundPreview.color;
            yield return null;
        }
    }
}