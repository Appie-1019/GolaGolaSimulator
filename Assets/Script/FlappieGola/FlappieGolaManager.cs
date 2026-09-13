using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class FlappieGolaManager : MonoBehaviour
{
    public static FlappieGolaManager Instance { get; private set; }
    public static bool GameOn;

    [Header("BG / Obj")]
    public BackgroundMovement[] allBG;
    public FlappieGolaPipeMovement pipe;
    public FlappieGolaPlayer player;
    [Header("Panel")]
    public GameObject controlInfoText;
    public RectTransform gameOverPanel;
    public TextMeshProUGUI gameOverTitleText;
    public string[] randomTitle;
    public TextMeshProUGUI gameOverScoreText;
    [Header("InGame")]
    public TextMeshProUGUI scoreText;
    public RectTransform scoreTextBg;
    [Header("Sound")]
    public AudioClip[] gameOverAudio;
    public AudioClip addPointAudio;

    private int score;

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
        StartCoroutine(GameReadyCoroutine());
        gameOverPanel.localScale = Vector3.zero;
    }

    IEnumerator GameReadyCoroutine()
    {
        ResetScore();
        pipe.canMove = false;
        pipe.Init();
        foreach (var bg in allBG)
        {
            bg.canMove = false;
        }

        controlInfoText.SetActive(true);
        player.FixPosition();

        yield return null;
        GameOn = true;
        yield return new WaitUntil(CheckJumpInput);

        controlInfoText.SetActive(false);

        pipe.canMove = true;
        foreach (var bg in allBG)
        {
            bg.canMove = true;
        }
    }

    private bool CheckJumpInput()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame) return true;
        else if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) return true;
        }

        return false;
    }

    public void GameOver()
    {
        AudioManager.Instance.PlayRandom2DSound(gameOverAudio,SoundType.Game);

        pipe.canMove = false;
        foreach (var bg in allBG)
        {
            bg.canMove = false;
        }

        player.rb.simulated = false;
        GameOn = false;
        gameOverPanel.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack);

        if (DataManager.SaveData.FlappieGola.bestScore < score)
        {
            gameOverScoreText.color = Color.yellow;
            DataManager.SaveData.FlappieGola.bestScore = score;
            DataManager.Save();
            gameOverScoreText.text = $"새로운 최고 기록! : {score:D4}";
        }
        else
        {
            gameOverScoreText.color = Color.white;
            gameOverScoreText.text = $"기록 : {score:D4}\r\n최고 기록 : {DataManager.SaveData.FlappieGola.bestScore:D4}";
        }
        
        gameOverTitleText.text = randomTitle[Random.Range(0, randomTitle.Length)];
    }

    public void RestartButton()
    {
        gameOverPanel.DOScale(Vector3.zero, 0.2f).SetEase(Ease.OutQuart);
        StartCoroutine(GameReadyCoroutine());
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = $"<color=orange>{score:D4}</color> 점";
    }

    public void AddScore(int value = 1)
    {
        score += value;
        scoreText.text = $"<color=orange>{score:D4}</color> 점";
        scoreTextBg.DOScale(Vector3.one * 1.2f, 0.0f);
        scoreTextBg.DOScale(Vector3.one, 0.2f);
        AudioManager.Instance.Play2DSound(addPointAudio, SoundType.Game);
    }
}
