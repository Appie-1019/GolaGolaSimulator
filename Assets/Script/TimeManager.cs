using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }
    
    public static float CustomDeltaTime;
    public static float CustomDeltaTimeFactor;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CustomDeltaTimeFactor = 1;
    }

    private void Update()
    {
        CustomDeltaTime = Time.deltaTime * CustomDeltaTimeFactor;
    }
}
