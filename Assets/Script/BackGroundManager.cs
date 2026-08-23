using UnityEngine;

public class BackGroundManager : MonoBehaviour
{
    public static BackGroundManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetBackgroundColor(Color color)
    {
        Camera.main.backgroundColor = color;
    }
}
