using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public MainUISaveData UI;
    public BackdoorSaveData Backdoor;
    public SaveVersion Version;

    public static SaveData Default => new SaveData
    {
        UI = new MainUISaveData
        {
            MasterVolume = 100.0f,
            GameVolume = 100.0f,
            UIVolume = 100.0f,
            ToastMessageAllow = true,
            GolaSoundAllow = true,
            GolaSoundPitchAllow = false,
        },

        Backdoor = new BackdoorSaveData
        {
            backdoorDialogIndex = 0
        },

        Version = new SaveVersion
        {
            Current = "1.5.2",
            All = new HashSet<string>()
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

[System.Serializable]
public struct BackdoorSaveData
{
    public int backdoorDialogIndex;
}

[System.Serializable]
public struct SaveVersion
{
    public string Current;

    public HashSet<string> All;
}