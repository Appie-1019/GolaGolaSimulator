using UnityEngine;

[CreateAssetMenu(fileName = "NewMusicData", menuName = "MusicStation/Music Data", order = 0)]
public class MusicData : ScriptableObject
{
    public AudioClip audioClip;
    public string title;
    public string artist;
    [TextArea(3, 10)]
    public string description;
    public Sprite coverImage;
    public Sprite discImage;
}