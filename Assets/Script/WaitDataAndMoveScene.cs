using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WaitDataAndMoveScene : MonoBehaviour
{
    [Tooltip("이동할 Scene의 이름")]
    public string targetSceneName;

    private void Start()
    {
        if (!GameManager.IsSceneInBuildSettings(targetSceneName))
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(WaitAndMoveCoroutine());
    }

    private IEnumerator WaitAndMoveCoroutine()
    {
        yield return new WaitUntil(() => DataManager.saveData != null);

        SceneManager.LoadScene(targetSceneName);
    }
}