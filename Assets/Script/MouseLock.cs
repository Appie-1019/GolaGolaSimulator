using UnityEngine;
using UnityEngine.InputSystem;

public class MouseLock : MonoBehaviour
{
    public static bool isMouseLocked = false;

    private void Start()
    {
        SetMouseLock(true);
    }

    public void OnMouseLock(InputValue value)
    {
        if (value.isPressed)
        {
            MouseLockToggle();
        }
    }

    private void MouseLockToggle() => SetMouseLock(!isMouseLocked);

    private void SetMouseLock(bool isLocked)
    {
        Cursor.visible = !isLocked;
        isMouseLocked = isLocked;
    }
}
