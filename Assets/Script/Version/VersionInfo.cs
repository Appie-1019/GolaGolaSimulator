using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;

public class VersionInfo : MonoBehaviour
{
    [Header("TMP")]
    public TextMeshProUGUI versionNameText;
    public TextMeshProUGUI developerCommentsText;

    [Header("Data")]
    public VersionDataSO[] versionData;
    public TextMeshProUGUI textSection;
    public Transform scrollRect;

    int currentIndex;
    List<TextMeshProUGUI> allText = new List<TextMeshProUGUI>();

    private void OnEnable()
    {
        Show(versionData.Length - 1);
    }

    private void Update()
    {
        if (!MenuPanelToggle.isPanelOpen) gameObject.SetActive(false);
    }

    public void MoveVersion(int move)
    {
        if (move == 0 || versionData == null || versionData.Length == 0) return;

        int targetIndex = (currentIndex + move) % versionData.Length;
        if (targetIndex < 0) targetIndex += versionData.Length;

        Show(targetIndex);
    }

    private void Show(int index)
    {
        if (index < 0 || index >= versionData.Length) return;
        RemoveAllText();
        currentIndex = index;
        VersionDataSO current = versionData[currentIndex];

        versionNameText.text = current.versionName;
        developerCommentsText.text = current.developerComment;

        if (current.additional != null && current.additional.Length > 0)
        {
            AddText("[추가됨]", Color.gold);
            for (int i = 0; i < current.additional.Length; i++)
            {
                AddUnorderedList(current.additional[i]);
            }
        }
        if (current.changes != null && current.changes.Length > 0)
        {
            AddText("[수정됨]", Color.gold);
            for (int i = 0; i < current.changes.Length; i++)
            {
                AddUnorderedList(current.changes[i]);
            }
        }
        if (current.etc != null && current.etc.Length > 0)
        {
            AddText("[기타]", Color.gold);
            for (int i = 0; i < current.etc.Length; i++)
            {
                AddUnorderedList(current.etc[i]);
            }
        }
    }

    private void AddUnorderedList(UnorderedList list, int indentation = 0)
    {
        string text = $"{MultiplyString(" ", indentation * 2)}- {list.item}";
        AddText(text);

        if (list.subItems != null && list.subItems.Length > 0)
        {
            for (int i = 0; i < list.subItems.Length; i++)
            {
                AddUnorderedList(list.subItems[i], indentation + 1);
            }
        }
    }

    private void AddText(string text)
    {
        AddText(text, Color.white);
    }

    private void AddText(string text, Color color)
    {
        TextMeshProUGUI newText = Instantiate(textSection, scrollRect);
        newText.text = text;
        newText.color = color;

        allText.Add(newText);
    }

    private void RemoveAllText()
    {
        if (allText == null && allText.Count == 0) return;

        for (int i = allText.Count - 1; i >= 0; i--)
        {
            Destroy(allText[i].gameObject);
        }

        allText.Clear();
    }

    public string MultiplyString(string source, int count)
    {
        if (string.IsNullOrEmpty(source) || count <= 0)
        {
            return string.Empty;
        }

        if (count == 1)
        {
            return source;
        }

        StringBuilder builder = new StringBuilder(source.Length * count);

        for (int i = 0; i < count; i++)
        {
            builder.Append(source);
        }

        return builder.ToString();
    }
}
