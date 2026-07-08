using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundPreview : MonoBehaviour
{
    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;
    public InputFieldGetValueFromSlider inputR;
    public InputFieldGetValueFromSlider inputG;
    public InputFieldGetValueFromSlider inputB;

    private Image image;
    [HideInInspector] public Color color;

    private void Awake()
    {
        image = GetComponent<Image>();
        if (image == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }
    }

    private void Start()
    {
        if (sliderR == null || sliderG == null || sliderB == null||
            inputR == null || inputG == null || inputB == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }

        sliderR.AddListener(OnValueChainged);
        sliderG.AddListener(OnValueChainged);
        sliderB.AddListener(OnValueChainged);
        inputR.AddListener(OnValueChainged);
        inputG.AddListener(OnValueChainged);
        inputB.AddListener(OnValueChainged);
        ChaingeImage();
    }

    private void OnValueChainged(float UselessValue)
    {
        ChaingeImage();
    }

    private void OnValueChainged()
    {
        ChaingeImage();
    }

    private void ChaingeImage()
    {
        Color newColor = new Color(sliderR.Value, sliderG.Value, sliderB.Value);
        color = image.color = newColor;
        BackGroundManager.Instance?.SetBackgroundColor(newColor);
    }

    public void DoARainbow()
    {
        StartCoroutine(RainbowBackGround());
    }

    IEnumerator RainbowBackGround()
    {
        float hue = 0f;

        while (true)
        {
            hue += Time.deltaTime;

            if (hue > 1f) hue -= 1f;

            Color nextColor = Color.HSVToRGB(hue, 1f, 1f);
            sliderR.Value = nextColor.r;
            sliderG.Value = nextColor.g;
            sliderB.Value = nextColor.b;
            color = nextColor;

            yield return null;
        }
    }
}
