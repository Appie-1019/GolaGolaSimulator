using UnityEngine;
using UnityEngine.UI;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public static bool CanToast { get; private set; }
    public static bool isMobile { get; private set; }

    [Header("Mobile UI Raycast Set")]
    [SerializeField] private Image[] mobileRaycastDisabledImages;
    [Header("Mobile Gameobject Enable/Disable Set")]
    [SerializeField] private GameObject[] mobileEnabledGameObjects;
    [SerializeField] private GameObject[] mobileDisabledGameObjects;
    [Header("Setting")]
    public ToggleSwitch toastToggleSwitch;
    //[Header("Debug")]
    //public bool moblie = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        isMobile = IsMobileDevice();
        DisableRaycastForMobile();
        DisableGameobjectForMoblie();
        EnableGameobjectForMoblie();
    }

    void Start()
    {
        toastToggleSwitch.AddToggleListener((isOn) => { CanToast = isOn; });
        CanToast = toastToggleSwitch.isEnable;
    }

    private void DisableRaycastForMobile()
    {
        if (mobileRaycastDisabledImages == null || mobileRaycastDisabledImages.Length == 0) return;

        foreach (Image img in mobileRaycastDisabledImages)
        {
            if (img != null) img.raycastTarget = !isMobile;
        }
    }

    private void DisableGameobjectForMoblie()
    {
        if (mobileDisabledGameObjects == null || mobileDisabledGameObjects.Length == 0) return;

        foreach (GameObject gameObject in mobileDisabledGameObjects)
        {
            if (gameObject != null) gameObject.SetActive(!isMobile);
        }
    }

    private void EnableGameobjectForMoblie()
    {
        if (mobileEnabledGameObjects == null || mobileEnabledGameObjects.Length == 0) return;

        foreach (GameObject gameObject in mobileEnabledGameObjects)
        {
            if (gameObject != null) gameObject.SetActive(isMobile);
        }
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
