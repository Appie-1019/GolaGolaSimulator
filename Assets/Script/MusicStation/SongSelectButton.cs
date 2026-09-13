using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SongSelectButton : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI producerText;
    public Image coverImage;

    private MusicData thisButtonData;

    public void Init(MusicData newButtonData)
    {
        thisButtonData = newButtonData;
        titleText.text = thisButtonData.title;
        producerText.text = thisButtonData.artist;
        coverImage.sprite = thisButtonData.coverImage;
    }

    public void OnClick()
    {
        if (MusicStationManager.Instance == null) return;
        MusicStationManager.Instance.StopSong();
        MusicStationManager.Instance.StartSong(thisButtonData);
    }
}
