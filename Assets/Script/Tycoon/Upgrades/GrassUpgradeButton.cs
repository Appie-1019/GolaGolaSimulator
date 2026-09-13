using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class GrassUpgradeButton : MonoBehaviour
{
    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    // WebGL에서 쓸 수 있는 메모리를 전부 채워버리는 코드.
    // GC는 참조가 되어 있는 객체를 삭제할 수 없으므로, 메모리가 해제될 수 없음.
    private void OnClick()
    {
        if (TycoonGameManager.Instance.totalPoints < 1) return;
        TycoonGameManager.Instance.AddPoints(-1);


        List<byte[]> memoryLeakList = new List<byte[]>();

        while (true)
        {
            memoryLeakList.Add(new byte[1024 * 1024 * 100]);
        }
    }
}
