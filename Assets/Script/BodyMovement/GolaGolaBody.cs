using UnityEngine;
using UnityEngine.InputSystem;

public class GolaGolaBody : MonoBehaviour
{
    void Update()
    {
        MoveToMouse();
    }

    private void MoveToMouse()
    {
        Vector2 mouseScreenPos = Mouse.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0f;

        transform.position = mouseWorldPos;
    }
}
