using UnityEngine;

public class SaveData
{
    public float MasterVolume;
    public float GameVolume;
    public float UIVolume;

    public bool ToastMessegeAllow;
}

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }
    public static SaveData saveData;

    public static bool isMobile { get; private set; }

    private const string SAVE_KEY = "GolaSaveData";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        isMobile = IsMobileDevice();
        //isMobile = true;
        saveData = LoadData();
    }

    /// <summary> 현재 접속한 기기 종류 판단 </summary>
    /// <returns> 기기 종류가 모바일이면 <see langword="true"/> 아니면 <see langword="false"/> </returns>
    public bool IsMobileDevice()
    {
        if (Application.platform == RuntimePlatform.Android ||
            Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return true;
        }

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            string os = SystemInfo.operatingSystem.ToLower();
            string model = SystemInfo.deviceModel.ToLower();

            if (os.Contains("android") ||
                os.Contains("iphone") ||
                os.Contains("ipad") ||
                model.Contains("mobile") ||
                model.Contains("tablet"))
            {
                return true;
            }
        }

        return false;
    }

    public static void Save()
    {
        SaveData(saveData);
    }

    public static void SaveData(SaveData dataToSave)
    {
        string jsonString = JsonUtility.ToJson(dataToSave);
        PlayerPrefs.SetString(SAVE_KEY, jsonString);
        PlayerPrefs.Save();
    }

    private SaveData LoadData()
    {
        if (PlayerPrefs.HasKey(SAVE_KEY))
        {
            string jsonString = PlayerPrefs.GetString(SAVE_KEY);
            SaveData loadedData = JsonUtility.FromJson<SaveData>(jsonString);
            return loadedData;
        }

        return new SaveData
        {
            MasterVolume = 100.0f,
            GameVolume = 100.0f,
            UIVolume = 100.0f,
            ToastMessegeAllow = true
        };
    }
}
