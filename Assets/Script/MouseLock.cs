using UnityEngine;

public class MouseLock : MonoBehaviour
{
    public static bool isMouseLocked = false;


    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.M))
        {
            SetMouseLock(!isMouseLocked);
        }
    }

    private void SetMouseLock(bool isLocked)
    {
        Cursor.visible = !isLocked;
        isMouseLocked = isLocked;

        string toastMessage = isLocked ? "마우스 보이지 않음" : "마우스 보임";
        ToastUIManager.Instance.AddToast(toastMessage);
    }
}
