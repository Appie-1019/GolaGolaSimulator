#if UNITY_EDITOR
// 주의 : AudioTestRunner.cs 는 모든 것을 AI(Gemini)에게 맡긴 코드임
using UnityEngine;
using UnityEngine.InputSystem;

public class AudioTestRunner : MonoBehaviour
{
    [Header("오디오 클립 설정")]
    public AudioClip clipK;
    public AudioClip clipL;

    [Header("재생 그룹 설정")]
    [Tooltip("인스펙터에서 재생할 오디오 그룹(Master, Game, UI)을 선택할 수 있습니다.")]
    public SoundType targetSoundType = SoundType.Game;

    [Header("믹서 그룹별 볼륨 조절 (0.0 ~ 1.0)")]
    [Range(0f, 1f)] public float masterVolume = 1f;
    [Range(0f, 1f)] public float gameVolume = 1f;
    [Range(0f, 1f)] public float uiVolume = 1f;

    private void Update()
    {
        // 최신 Input System의 키보드 연결 상태 확인
        if (Keyboard.current == null) return;

        // K 키를 눌렀을 때
        if (Keyboard.current.kKey.wasPressedThisFrame)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play3DSound(clipK, new Vector3(10,0,0), targetSoundType);
            }
        }

        // L 키를 눌렀을 때
        if (Keyboard.current.lKey.wasPressedThisFrame)
        {
            if (AudioManager.Instance != null)
            {
                AudioManager.Instance.Play2DSound(clipL, targetSoundType);
            }
        }
    }

    // 인스펙터에서 값이 변경될 때마다 자동으로 호출되는 유니티 내장 함수입니다.
    private void OnValidate()
    {
        // 게임이 실행 중일 때만 볼륨 조절을 수행합니다.
        if (Application.isPlaying && AudioManager.Instance != null)
        {
            // 3개의 볼륨 값을 각각의 SoundType에 맞추어 믹서에 실시간으로 전달합니다.
            AudioManager.Instance.SetVolume(masterVolume, SoundType.Master);
            AudioManager.Instance.SetVolume(gameVolume, SoundType.Game);
            AudioManager.Instance.SetVolume(uiVolume, SoundType.UI);
        }
    }
}
#endif