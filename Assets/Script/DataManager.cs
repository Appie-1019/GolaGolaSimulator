using UnityEngine;
using UnityEngine.InputSystem;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public static bool CanToast { get; private set; }
    public static bool isMobile { get; private set; }

    public ToggleSwitch toastToggleSwitch;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        //isMobile = true;
        isMobile = IsMobileDevice();
    }

    void Start()
    {
        toastToggleSwitch.AddToggleListener((isOn) => { CanToast = isOn; });
        CanToast = toastToggleSwitch.isEnable;
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
}
