[System.Serializable]
public class SaveData
{
    public MainUISaveData UI;
    public BackdoorSaveData Backdoor;
    public SaveVersion Version;
    public TycoonData Tycoon;
    public FlappieGolaData FlappieGola;

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
            RandomGolaSoundPitchAllow = false
        },

        Backdoor = new BackdoorSaveData
        {
            backdoorDialogIndex = 0
        },

        Version = new SaveVersion
        {
            Current = "1.7"
        },

        Tycoon = new TycoonData
        {
            bestTime = -1019.0f
        },

        FlappieGola = new FlappieGolaData
        {
            bestScore = 0
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
    public bool RandomGolaSoundPitchAllow;
}

[System.Serializable]
public struct BackdoorSaveData
{
    public int backdoorDialogIndex;
}

[System.Serializable]
public struct TycoonData
{
    public float bestTime;
}

[System.Serializable]
public struct FlappieGolaData
{
    public int bestScore;
}

[System.Serializable]
public struct SaveVersion
{
    public string Current;
}