using UnityEngine;

public class VSyncSet : MonoBehaviour
{
    void Start()
    {
        QualitySettings.vSyncCount = 1;
    }
}
