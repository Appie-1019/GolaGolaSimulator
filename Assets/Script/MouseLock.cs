using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLock : MonoBehaviour
{
    public static bool isMouseLocked = false;

    private float holdTimer = 0f;
    private bool hasToggledThisPress = false;
    private readonly float requiredHoldTime = 0.478723f;

    private void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.mKey.isPressed)
        {
            if (!hasToggledThisPress)
            {
                holdTimer += Time.deltaTime;

                if (holdTimer >= requiredHoldTime)
                {
                    SetMouseLock(!isMouseLocked);
                    hasToggledThisPress = true;
                }
            }
        }
        else
        {
            holdTimer = 0f;
            hasToggledThisPress = false;
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