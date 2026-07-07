using UnityEngine;
using UnityEngine.UI;

public class BackgroundPreview : MonoBehaviour
{
    public Slider sliderR;
    public Slider sliderG;
    public Slider sliderB;
    private Image image;

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
        if (sliderR == null || sliderG == null || sliderB == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }

        sliderR.AddListener(OnValueChainged);
        sliderG.AddListener(OnValueChainged);
        sliderB.AddListener(OnValueChainged);
        ChaingeImage();
    }

    private void OnValueChainged(float UselessValue)
    {
        ChaingeImage();
    }

    private void ChaingeImage()
    {
        Color newColor = new Color(sliderR.Value, sliderG.Value, sliderB.Value);
        image.color = newColor;
        BackGroundManager.Instance?.SetBackgroundColor(newColor);
    }
}
