using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance { get; private set; }

    public static bool CanToast { get; private set; }

    public ToggleSwitch toastToggleSwitch;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        toastToggleSwitch.AddToggleListener((isOn) => { CanToast = isOn; });
        CanToast = toastToggleSwitch.isEnable;
    }
}
