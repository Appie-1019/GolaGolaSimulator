using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLock : MonoBehaviour
{
    public static bool isMouseLocked = false;

    private void Update()
    {
        if(Keyboard.current != null && Keyboard.current.mKey.wasPressedThisFrame)
        {
            SetMouseLock(!isMouseLocked);
        }
    }

    private void SetMouseLock(bool isLocked)
    {
        Cursor.visible = !isLocked;
        isMouseLocked = isLocked;

        if (ToastUIManager.Instance == null) return;
        string toastMessage = isLocked ? "마우스 보이지 않음" : "마우스 보임";
        ToastUIManager.Instance.AddToast(toastMessage);
    }
}
