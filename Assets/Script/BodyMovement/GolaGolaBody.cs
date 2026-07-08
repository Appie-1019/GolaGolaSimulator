using UnityEngine;
using UnityEngine.InputSystem;

public class GolaGolaBody : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 10f;

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

        Vector3 targetPos = GetPointerPos();
        transform.position = SmoothTo(transform.position, targetPos);
    }

    public void MobileControl()
    {
        if (Pointer.current.press.wasPressedThisFrame)
        {
            offset = transform.position - GetPointerPos();
        }

        if (Pointer.current.press.isPressed)
        {
            Vector3 targetPos = GetPointerPos() + offset;
            transform.position = SmoothTo(transform.position, targetPos);
        }

        if (Touchscreen.current == null) return;

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
            }
            transform.position = SmoothTo(transform.position, originalPos);
        }
        else
        {
            isTwoFingersPressed = false;
        }
    }

    private Vector3 SmoothTo(Vector3 current, Vector3 target)
    {
        float t = 1f - Mathf.Exp(-moveSpeed * Time.deltaTime);
        return Vector3.Lerp(current, target, t);
    }

    Vector3 GetPointerPos()
    {
        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }
}