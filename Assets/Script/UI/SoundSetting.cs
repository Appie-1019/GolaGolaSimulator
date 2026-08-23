using UnityEngine;

public class SoundSetting : MonoBehaviour
{
    [Header("Slider")]
    public Slider masterVolumeSlider;
    public Slider gameVolumeSlider;
    public Slider UIVolumeSlider;
    [Header("Toggle")]
    public ToggleSwitch golaSoundSwitch;
    public ToggleSwitch golaSoundPitchSwitch;

    void Awake()
    {
        masterVolumeSlider.AddListener(SetMasterVolume);
        gameVolumeSlider.AddListener(SetGameVolume);
        UIVolumeSlider.AddListener(SetUIVolume);
        golaSoundSwitch.AddToggleListener(SetGolaSoundEnable);
        golaSoundPitchSwitch.AddToggleListener(SetGolaSoundPitchEnable);
    }

    private void Start()
    {
        masterVolumeSlider.Value = DataManager.saveData.UI.MasterVolume;
        gameVolumeSlider.Value = DataManager.saveData.UI.GameVolume;
        UIVolumeSlider.Value = DataManager.saveData.UI.UIVolume;
        golaSoundSwitch.SetEnable(DataManager.saveData.UI.GolaSoundAllow, true);
        golaSoundPitchSwitch.SetEnable(DataManager.saveData.UI.GolaSoundPitchAllow, true);
    }

    private void SetMasterVolume(float volume)
    {
        AudioManager.Instance?.SetVolume(volume / 100, SoundType.Master);
        DataManager.saveData.UI.MasterVolume = masterVolumeSlider.Value;
        DataManager.Save();
    }

    private void SetGameVolume(float volume)
    {
        AudioManager.Instance?.SetVolume(volume / 100, SoundType.Game);
        DataManager.saveData.UI.GameVolume = gameVolumeSlider.Value;
        DataManager.Save();
    }

    private void SetUIVolume(float volume)
    {
        AudioManager.Instance?.SetVolume(volume / 100, SoundType.UI);
        DataManager.saveData.UI.UIVolume = UIVolumeSlider.Value;
        DataManager.Save();
    }

    private void SetGolaSoundEnable(bool enable)
    {
        DataManager.saveData.UI.GolaSoundAllow = enable;
        DataManager.Save();
    }

    private void SetGolaSoundPitchEnable(bool enable)
    {
        DataManager.saveData.UI.GolaSoundPitchAllow = enable;
        DataManager.Save();
    }
}
