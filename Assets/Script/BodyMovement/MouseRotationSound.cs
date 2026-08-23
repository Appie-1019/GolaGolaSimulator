using UnityEngine;

public class MouseRotationSound : MonoBehaviour
{
    [Header("Pointer")]
    public Transform pointer;

    [Header("Time Settings")]
    public float minRotationTime = 0.5f; // 이 시간보다 짧거나 같으면 정규화 값 1 (가장 빠름)
    public float maxRotationTime = 2.0f; // 이 시간보다 길거나 같으면 정규화 값 0 (가장 느림)

    [Header("Sounds")]
    public AudioClip[] sounds;


    private float previousAngle = 0f;
    private float accumulatedAngle = 0f;
    private float lastRotationTime = 0f;

    private void Start()
    {
        if (pointer != null)
        {
            previousAngle = GetAngleFromOrigin(pointer.position);
        }
        lastRotationTime = Time.time;
    }

    private void Update()
    {
        if (!DataManager.saveData.UI.GolaSoundAllow) return;
        if (pointer == null) return;
        if (MovementManager.Instance == null || MovementManager.Instance.currentType == MovementType.AppieSlide) return;

        float currentAngle = GetAngleFromOrigin(pointer.position);
        float deltaAngle = Mathf.DeltaAngle(previousAngle, currentAngle);

        accumulatedAngle += deltaAngle;

        if (Mathf.Abs(accumulatedAngle) >= 360f)
        {
            float timeSinceLastRotation = Time.time - lastRotationTime;
            float normalizedSpeed = Mathf.InverseLerp(maxRotationTime, minRotationTime, timeSinceLastRotation);
            OnFullRotationDetected(normalizedSpeed);

            accumulatedAngle -= Mathf.Sign(accumulatedAngle) * 360f;
            lastRotationTime = Time.time;
        }

        previousAngle = currentAngle;
    }

    private float GetAngleFromOrigin(Vector3 position)
    {
        return Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
    }

    private void OnFullRotationDetected(float normalizedSpeed)
    {
        if (AudioManager.Instance == null) return;
        if (DataManager.saveData.UI.GolaSoundPitchAllow) normalizedSpeed = Mathf.Max(0.4f, normalizedSpeed) * 1.4f;
        else normalizedSpeed = 1;
        AudioManager.Instance.PlayRandom2DSound(sounds, SoundType.Game, 1, normalizedSpeed);
    }
}
