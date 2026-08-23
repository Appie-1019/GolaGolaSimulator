using System;
using TMPro;
using UnityEngine;

[Serializable]
public struct SwitchBoxSection
{
    public int SectionID;
    public string DisplayName;
}

public class SwitchBox : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private SwitchBoxSection[] sections;
    [Header("Parts")]
    [SerializeField] private TextMeshProUGUI Text;

    System.Action<int> intToggleListener;
    System.Action<SwitchBoxSection> switchBoxDataToggleListener;
    SwitchBoxSection currentData;
    int index;

    private void Reset()
    {
        sections = new SwitchBoxSection[]
        {
            new SwitchBoxSection { SectionID = 0, DisplayName = "Section Name" }
        };
    }

    private void Start()
    {
        if (sections == null || sections.Length == 0)
        {
            Debug.LogError(gameObject.name + "은(는) 필요한 데이터 배열이 없음");
            this.enabled = false;
            return;
        }

        SetSection(sections[0]);
        index = 0;
    }

    public void NextSection()
    {
        index++;
        if (index >= sections.Length) index = 0;

        SetSection(sections[index]);
    }

    public void PreviousSection()
    {
        index--;
        if (index < 0) index = sections.Length - 1;

        SetSection(sections[index]);
    }

    void SetSection(SwitchBoxSection newData)
    {
        currentData = newData;
        Text.text = newData.DisplayName;

        intToggleListener?.Invoke(newData.SectionID);
        switchBoxDataToggleListener?.Invoke(newData);
    }

    public void AddToggleListener(System.Action<int> listener) => intToggleListener += listener;
    public void RemoveToggleListener(System.Action<int> listener) => intToggleListener -= listener;
    public void AddToggleListener(System.Action<SwitchBoxSection> listener) => switchBoxDataToggleListener += listener;
    public void RemoveToggleListener(System.Action<SwitchBoxSection> listener) => switchBoxDataToggleListener -= listener;
}