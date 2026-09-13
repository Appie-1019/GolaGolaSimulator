using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MusicStationManager : MonoBehaviour
{
    public static MusicStationManager Instance { get; private set; }

    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI producerText;
    public TextMeshProUGUI descriptionText;
    public Image coverImage;
    public Image discImage;

    //[Header("PauseButton")]
    //public Image pauseButtonImage;
    //public Sprite pauseImage;
    //public Sprite playImage;

    [Header("Animation")]
    public Animator anim;
    public float discSpinSpeed;

    private MusicData currentMusicData;
    private AudioInstance currentMusic;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Update()
    {
        if (currentMusic == null || discImage == null) return;
        if (currentMusic.isActiveAndEnabled)
        {
            discImage.rectTransform.Rotate(Vector3.forward * (-discSpinSpeed * Time.deltaTime));
        }
    }

    public void StopSong()
    {
        if (currentMusic == null) return;
        currentMusic.StopSound(0.2f);
        currentMusic = null;
    }

    public void StartSong(MusicData data)
    {
        currentMusicData = data;
        anim.SetTrigger("Switch");
    }

    public void SetUI()
    {
        titleText.text = WrapWithAngleBrackets(currentMusicData.title);
        producerText.text = $"By <color=orange>{currentMusicData.artist}</color>";
        descriptionText.text = currentMusicData.description;
        coverImage.sprite = currentMusicData.coverImage;
        discImage.sprite = currentMusicData.discImage;
        discImage.rectTransform.rotation = Quaternion.identity;
        currentMusic = AudioManager.Instance?.Play2DSound(currentMusicData.audioClip, SoundType.Master);
    }

    public static string WrapWithAngleBrackets(string input)
    {
        if (input == null) return "〈〉";

        int length = input.Length;
        if (length == 0) return "〈〉";

        return string.Create(length + 2, input, (span, str) =>
        {
            span[0] = '〈';
            str.AsSpan().CopyTo(span.Slice(1));
            span[span.Length - 1] = '〉';
        });
    }

    //public void TogglePause()
    //{
    //    if (currentMusic == null) return;
    //    if (currentMusic.IsPaused)
    //    {
    //        Resume();
    //    }
    //    else
    //    {
    //        Pause();
    //    }
    //}

    //private void Resume()
    //{
    //    currentMusic.ResumeSound();
    //    pauseButtonImage.sprite = playImage;
    //}

    //private void Pause()
    //{
    //    currentMusic.PauseSound();
    //    pauseButtonImage.sprite = pauseImage;
    //}
}
