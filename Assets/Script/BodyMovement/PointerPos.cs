using UnityEngine;
using UnityEngine.InputSystem;

public class PointerPos : MonoBehaviour
{
    public static PointerPos Instance { get; private set; }

    Vector3 offset;
    Vector3 originalPos;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        originalPos = transform.position;
    }

    public void UpdatePosition()
    {
        if (DataManager.isMobile) MobileControl();
        else MouseControl();
    }

    private void MouseControl()
    {
        if (Pointer.current == null) return;

        Vector3 targetPos = GetPointerPos();
        targetPos = Vector3.ClampMagnitude(targetPos, 20f);
        transform.position = targetPos;
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
                Vector3 targetPosition = GetPointerPos() + offset;
                transform.position = Vector3.ClampMagnitude(targetPosition, 20f);
            }
        }
    }

    Vector3 GetPointerPos()
    {
        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }

    public void ResetPosition()
    {
        transform.position = originalPos;
    }
}
