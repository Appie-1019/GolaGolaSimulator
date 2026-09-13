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
    public ToggleSwitch randomGolaSoundPitchSwitch;

    void Awake()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.AddListener(SetMasterVolume);
        if (gameVolumeSlider != null) gameVolumeSlider.AddListener(SetGameVolume);
        if (UIVolumeSlider != null) UIVolumeSlider.AddListener(SetUIVolume);
        if (golaSoundSwitch != null) golaSoundSwitch.AddToggleListener(SetGolaSoundEnable);
        if (golaSoundPitchSwitch != null) golaSoundPitchSwitch.AddToggleListener(SetGolaSoundPitchEnable);
        if (randomGolaSoundPitchSwitch != null) randomGolaSoundPitchSwitch.AddToggleListener(SetRandomGolaSoundPitchEnable);
    }

    private void Start()
    {
        if (masterVolumeSlider != null) masterVolumeSlider.Value = DataManager.SaveData.UI.MasterVolume;
        if (gameVolumeSlider != null) gameVolumeSlider.Value = DataManager.SaveData.UI.GameVolume;
        if (UIVolumeSlider != null) UIVolumeSlider.Value = DataManager.SaveData.UI.UIVolume;
        if (golaSoundSwitch != null) golaSoundSwitch.SetEnable(DataManager.SaveData.UI.GolaSoundAllow, true);
        if (golaSoundPitchSwitch != null) golaSoundPitchSwitch.SetEnable(DataManager.SaveData.UI.GolaSoundPitchAllow, true);
        if (randomGolaSoundPitchSwitch != null) randomGolaSoundPitchSwitch.SetEnable(DataManager.SaveData.UI.RandomGolaSoundPitchAllow, true);
    }

    private void SetMasterVolume(float volume)
    {
        AudioManager.Instance?.SetVolume(volume / 100, SoundType.Master);
        DataManager.SaveData.UI.MasterVolume = masterVolumeSlider.Value;
        DataManager.Save();
    }

    private void SetGameVolume(float volume)
    {
        AudioManager.Instance?.SetVolume(volume / 100, SoundType.Game);
        DataManager.SaveData.UI.GameVolume = gameVolumeSlider.Value;
        DataManager.Save();
    }

    private void SetUIVolume(float volume)
    {
        AudioManager.Instance?.SetVolume(volume / 100, SoundType.UI);
        DataManager.SaveData.UI.UIVolume = UIVolumeSlider.Value;
        DataManager.Save();
    }

    private void SetGolaSoundEnable(bool enable)
    {
        DataManager.SaveData.UI.GolaSoundAllow = enable;
        DataManager.Save();
    }

    private void SetGolaSoundPitchEnable(bool enable)
    {
        DataManager.SaveData.UI.GolaSoundPitchAllow = enable;
        DataManager.Save();
    }

    private void SetRandomGolaSoundPitchEnable(bool enable)
    {
        DataManager.SaveData.UI.RandomGolaSoundPitchAllow = enable;
        DataManager.Save();
    }
}
