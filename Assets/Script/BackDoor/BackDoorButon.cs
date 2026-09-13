using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button), typeof(Image))]
public class BackDoorButon : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Frame")]
    public Sprite[] buttonImageFrame;
    public float frameDuration = 0.1f;
    [Header("Tooltip")]
    public GameObject tooltipObject;
    [Header("Scene Info")]
    public string sceneName;

    private Button button;
    private Image image;

    void Awake()
    {
        button = GetComponent<Button>();
        image = GetComponent<Image>();

        button.onClick.AddListener(OnButtonClicked);
    }

    private void Start()
    {
        BackdoorTextPrinter.Instance.sceneButtons.Add(this);
        gameObject.SetActive(false);
    }

    private void OnButtonClicked()
    {
        BackdoorTextPrinter.Instance.GoToScene(sceneName);
    }

    IEnumerator ButtonFrame()
    {
        int frameCount = buttonImageFrame.Length;
        int currentFrame = 0;
        while (true)
        {
            image.sprite = buttonImageFrame[currentFrame];
            currentFrame = (currentFrame + 1) % frameCount;
            yield return new WaitForSeconds(frameDuration);
        }
    }

    private void OnEnable()
    {
        StartCoroutine(ButtonFrame());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (tooltipObject != null) tooltipObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (tooltipObject != null) tooltipObject.SetActive(false);
    }
}
