using UnityEngine;

public class SoundSetting : MonoBehaviour
{
    [Header("Slider")]
    public Slider masterVolumeSlider;
    public Slider gameVolumeSlider;
    public Slider UIVolumeSlider;

    void Awake()
    {
        masterVolumeSlider.AddListener(SetMasterVolume);
        gameVolumeSlider.AddListener(SetGameVolume);
        UIVolumeSlider.AddListener(SetUIVolume);
    }

    private void Start()
    {
        masterVolumeSlider.Value = DataManager.saveData.MasterVolume;
        gameVolumeSlider.Value = DataManager.saveData.GameVolume;
        UIVolumeSlider.Value = DataManager.saveData.UIVolume;
    }

    private void SetMasterVolume(float volume)
    {
        AudioManager.Instance?.SetVolume(volume / 100, SoundType.Master);
        DataManager.saveData.MasterVolume = masterVolumeSlider.Value;
        DataManager.Save();
    }

    private void SetGameVolume(float volume)
    {
        AudioManager.Instance?.SetVolume(volume / 100, SoundType.Game);
        DataManager.saveData.GameVolume = gameVolumeSlider.Value;
        DataManager.Save();
    }

    private void SetUIVolume(float volume)
    {
        AudioManager.Instance?.SetVolume(volume / 100, SoundType.UI);
        DataManager.saveData.UIVolume = UIVolumeSlider.Value;
        DataManager.Save();
    }
}
