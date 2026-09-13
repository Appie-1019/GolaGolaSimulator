using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class TycoonGameManager : MonoBehaviour
{
    public static TycoonGameManager Instance { get; private set; }

    [Header("points")]
    public Int6D totalPoints;
    public Int6D incrementValue;
    [Header("UI")]
    public TextMeshProUGUI totalPointText;
    public TextMeshProUGUI remainingValueText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI bestTimeText;
    public GameObject endingPanel;

    [Header("Pointer / body")]
    public PointerPos pointer;
    public GolaGolaBody body;
    public GolaGolaParts[] parts;

    [Header("Rotation Time Settings")]
    public float minRotationTime = 0.5f; // 이 시간보다 짧거나 같으면 정규화 값 1 (가장 빠름)
    public float maxRotationTime = 2.0f; // 이 시간보다 길거나 같으면 정규화 값 0 (가장 느림)

    [Header("Rotation Sounds")]
    public AudioClip[] sounds;

    private float previousAngle = 0f;
    private float accumulatedAngle = 0f;
    private float lastRotationTime = 0f;
    private float elapsedTime = 0f;
    private bool gameEnd = false;
    private Coroutine timerCoroutine;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        totalPoints = 0;
        incrementValue = 1;
        UpdateUI();
        UpdateBestTimeUI();
    }

    private void Update()
    {
        if (gameEnd) return;
        MoveBody();
        RotationCheck();

        //if (Keyboard.current.kKey.wasPressedThisFrame)
        //{
        //    totalPoints = Int6D.MaxValue - 50;
        //}
    }

    void MoveBody()
    {
        pointer.UpdatePosition();
        body.GotoPointer();

        foreach (GolaGolaParts part in parts)
        {
            part.LookAtBody();
        }
    }

    void RotationCheck()
    {
        if (!DataManager.SaveData.UI.GolaSoundAllow) return;
        if (pointer == null) return;

        float currentAngle = GetAngleFromOrigin(pointer.transform.position);
        float deltaAngle = Mathf.DeltaAngle(previousAngle, currentAngle);

        accumulatedAngle += deltaAngle;

        if (Mathf.Abs(accumulatedAngle) >= 360f)
        {
            float timeSinceLastRotation = Time.time - lastRotationTime;
            float normalizedSpeed = Mathf.InverseLerp(maxRotationTime, minRotationTime, timeSinceLastRotation);
            accumulatedAngle -= Mathf.Sign(accumulatedAngle) * 360f;
            lastRotationTime = Time.time;

            if (totalPoints.IsZero) StartTimer();
            OnFullRotationDetected(normalizedSpeed);
            AddPoints(incrementValue);
        }

        previousAngle = currentAngle;
    }

    public void AddPoints(Int6D value, bool updateUI = true)
    {
        totalPoints += value;
        if (totalPoints < 0) totalPoints = 0;
        if (updateUI) UpdateUI();
        CheckClear();
    }

    public void MultiplierPoint(Int6D value, bool updateUI = true)
    {
        totalPoints *= value;
        if (totalPoints < 0) totalPoints = 0;
        if (updateUI) UpdateUI();
        CheckClear();
    }

    public void SetPoint(Int6D value, bool updateUI = true)
    {
        totalPoints = value;
        if (totalPoints < 0) totalPoints = 0;
        if (updateUI) UpdateUI();
        CheckClear();
    }

    public void CheckClear()
    {
        if (totalPoints == Int6D.MaxValue)
        {
            gameEnd = true;
            endingPanel.SetActive(true);
            StopTimer();
            TryUpdateBestTime(elapsedTime);
        }
    }

    private float GetAngleFromOrigin(Vector3 position)
    {
        return Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
    }

    private void OnFullRotationDetected(float normalizedSpeed)
    {
        ParticleTextManager.Instance.AddParticleText();

        if (AudioManager.Instance == null) return;
        normalizedSpeed = Mathf.Max(0.4f, normalizedSpeed) * 1.4f;
        AudioManager.Instance.PlayRandom2DSound(sounds, SoundType.Game, 1, normalizedSpeed);
    }

    public void UpdateUI()
    {
        totalPointText.text = $"<color=orange>{totalPoints}</color> GJ";
        remainingValueText.text = $"완료하려면 <color=orange>{Int6D.MaxValue - totalPoints} GJ</color> 더 필요함 lol ez gg p2w";
    }

    private void TryUpdateBestTime(float newTime)
    {
        if (DataManager.Instance == null) return;
        if (DataManager.SaveData.Tycoon.bestTime > 0 &&
            DataManager.SaveData.Tycoon.bestTime < newTime)
        {
            timerText.text += $" (최고 기록 + {newTime - DataManager.SaveData.Tycoon.bestTime:F4})";
            return;
        }

        float prevBestTime = DataManager.SaveData.Tycoon.bestTime;

        DataManager.SaveData.Tycoon.bestTime = newTime;
        DataManager.Save();

        int minutes = (int)(DataManager.SaveData.Tycoon.bestTime / 60);
        int seconds = (int)(DataManager.SaveData.Tycoon.bestTime % 60);

        float fraction = DataManager.SaveData.Tycoon.bestTime % 1f;
        int truncatedFraction = (int)(fraction * 10000);

        bestTimeText.text = $"최고기록 : {minutes:D2}:{seconds:D2}.{truncatedFraction:D4}" + (prevBestTime <= 0 ? " (첫 번째 클리어!)": $" ({prevBestTime - newTime:F4}초 단축!)");
        bestTimeText.color = Color.orange;
    }

    public void UpdateBestTimeUI()
    {
        if (DataManager.Instance != null)
        {
            if (DataManager.SaveData.Tycoon.bestTime > 0.0f)
            {
                int minutes = (int)(DataManager.SaveData.Tycoon.bestTime / 60);
                int seconds = (int)(DataManager.SaveData.Tycoon.bestTime % 60);

                float fraction = DataManager.SaveData.Tycoon.bestTime % 1f;
                int truncatedFraction = (int)(fraction * 10000);

                bestTimeText.text = $"최고기록 : {minutes:D2}:{seconds:D2}.{truncatedFraction:D4}";

                return;
            }
        }

        bestTimeText.text = "최고기록 : (없음)";
    }

    public void StartTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
        }
        timerCoroutine = StartCoroutine(UpdateTimerCoroutine());
    }

    public void StopTimer()
    {
        if (timerCoroutine != null)
        {
            StopCoroutine(timerCoroutine);
            timerCoroutine = null;
        }
    }

    private IEnumerator UpdateTimerCoroutine()
    {
        while (true)
        {
            elapsedTime += Time.deltaTime;

            int minutes = (int)(elapsedTime / 60);
            int seconds = (int)(elapsedTime % 60);

            float fraction = elapsedTime % 1f;
            int truncatedFraction = (int)(fraction * 10000);

            timerText.text = $"기록 : {minutes:D2}:{seconds:D2}.{truncatedFraction:D4}";

            yield return null;
        }
    }
}
