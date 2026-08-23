using UnityEngine;
using UnityEngine.InputSystem;

public class GolaGolaBody : MonoBehaviour
{
    Vector3 offset;
    bool isTwoFingersPressed;
    Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.position;
    }

    void Update()
    {
        if (DataManager.isMobile) MobileControl();
        else MoveToMouse();
    }

    private void MoveToMouse()
    {
        if (Pointer.current == null) return;
        
        transform.position = GetPointerPos();
    }

    public void MobileControl()
    {
        if (Pointer.current.press.wasPressedThisFrame)
        {
            offset = transform.position - GetPointerPos();
        }

        if (Pointer.current.press.isPressed)
        {
            Vector2 delta = Pointer.current.delta.ReadValue();
            if (delta.sqrMagnitude > 0.01f)
            {
                transform.position = GetPointerPos() + offset;
            }
        }

        if (Touchscreen.current == null) return;

        var allTouches = Touchscreen.current.touches;
        int activeTouchCount = 0;

        foreach (var touch in Touchscreen.current.touches)
        {
            if (touch.press.isPressed) activeTouchCount++;
        }

        if (activeTouchCount == 2)
        {
            if (!isTwoFingersPressed)
            {
                isTwoFingersPressed = true;
                transform.position = originalPos;
            }
        }
        else
        {
            isTwoFingersPressed = false;
        }
    }

    Vector3 GetPointerPos()
    {
        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }
}
