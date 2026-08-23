using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Button))]
public class SceneChangeButton : MonoBehaviour
{
    [Header("Scene name")]
    [SerializeField] private string targetSceneName;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();

        if (button != null)
        {
            button.onClick.AddListener(OnButtonClicked);
        }
    }

    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClicked);
        }
    }

    private void OnButtonClicked()
    {
        if (!GameManager.TryLoadScene(targetSceneName))
        {
            Debug.LogWarning("이동할 씬 이름이 올바르지 않음");
        }
    }
}