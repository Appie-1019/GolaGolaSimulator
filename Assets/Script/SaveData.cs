[System.Serializable]
public class SaveData
{
    public string VersionName;
    public MainUISaveData UI;

    public static readonly string Version = "1.5.1";
    public static SaveData Default => new SaveData
    {
        VersionName = Version,
        UI = new MainUISaveData
        {
            MasterVolume = 100.0f,
            GameVolume = 100.0f,
            UIVolume = 100.0f,
            ToastMessageAllow = true,
            GolaSoundAllow = true,
            GolaSoundPitchAllow = false,
        }
    };
}

[System.Serializable]
public struct MainUISaveData
{
    public float MasterVolume;
    public float GameVolume;
    public float UIVolume;

    public bool ToastMessageAllow;
    public bool GolaSoundAllow;
    public bool GolaSoundPitchAllow;
}