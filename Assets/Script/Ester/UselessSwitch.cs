using System.Collections;
using TMPro;
using UnityEngine;
public class UselessSwitch : MonoBehaviour
{
    public ToggleSwitch toggleSwitch;
    int toggleCount = 0;

    TextMeshProUGUI tmp;
    Coroutine rainbowTextCoroutine;

    private void Awake()
    {
        tmp = GetComponent<TextMeshProUGUI>();

        if (toggleSwitch == null || tmp == null)
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
            tmp.text = "GolaGola?";
            if (rainbowTextCoroutine != null) StopCoroutine(rainbowTextCoroutine);
            rainbowTextCoroutine = StartCoroutine(RainbowTMP());
            if (BackgroundManager.Instance != null) BackgroundManager.Instance.DoARainbow();
        }
        else if (toggleCount == 40)
        {
            tmp.text = "인생에 전혀 필요 없는 스위치";
            tmp.color = Color.white;
            toggleCount = 0;
            StopCoroutine(rainbowTextCoroutine);
            rainbowTextCoroutine = null;
            if (BackgroundManager.Instance != null) BackgroundManager.Instance.StopARainbow();
        }
    }

    IEnumerator RainbowTMP()
    {
        while (true)
        {
            if (BackgroundManager.Instance != null) tmp.color = BackgroundManager.Instance.backgroundColor;
            yield return null;
        }
    }
}