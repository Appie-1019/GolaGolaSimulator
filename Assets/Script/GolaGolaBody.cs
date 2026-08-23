using UnityEngine;
using UnityEngine.InputSystem;

public class GolaGolaBody : MonoBehaviour
{
    void Update()
    {
        if (MouseLock.isMouseLocked) MoveToMouse();
    }

    private void MoveToMouse()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        transform.position = mouseWorldPos;
    }
}
