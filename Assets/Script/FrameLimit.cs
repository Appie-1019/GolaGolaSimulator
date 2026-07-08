using UnityEngine;

public class FrameLimit : MonoBehaviour
{
    void Start()
    {
        // 유니티 에디터(개발 환경)일 때만 실행
#if UNITY_EDITOR
        QualitySettings.vSyncCount = 0; // 수직동기화 해제
        Application.targetFrameRate = 60; // 60프레임 제한
#endif
    }
}
