using UnityEngine;

[CreateAssetMenu(fileName = "New Version Info", menuName = "Version Data/New Version Data")]
public class VersionDataSO : ScriptableObject
{
    [Header("Version")]
    public string versionName;

    [Header("Developer Comments")]
    [TextArea(3, 10)]
    public string developerComment;

    [Header("Detail")]
    public UnorderedList[] additional;
    public UnorderedList[] changes;
    public UnorderedList[] etc;
}

[System.Serializable]
public struct UnorderedList
{
    public string item;
    public UnorderedList[] subItems;
}