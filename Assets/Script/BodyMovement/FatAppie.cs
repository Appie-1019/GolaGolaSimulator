using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class FatAppie : MonoBehaviour
{
    public float lunchPower;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("WHAT??? HOW???");
        }
    }

    private void OnEnable()
    {
        transform.position = Vector3.zero;
        string infoText = DataManager.isMobile ? "뚱뚱남 : 터치해서 날려보내기" : "뚱뚱남 : 클릭해서 날려보내기";
        ToastUIManager.Instance?.AddToast(infoText, Color.yellow);
    }

    void Update()
    {
        if (TryGetClickPosition(out Vector3 targetPos))
        {
            Vector3 dir = (targetPos - transform.position).normalized;
            rb.linearVelocity = dir * lunchPower;
        }

        if (transform.position.magnitude >= 20)
        {
            transform.position = Vector3.zero;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0.0f;
            ToastUIManager.Instance.AddToast("너무 빨라서 경계를 넘어섰습니다!");
        }
    }

    public bool TryGetClickPosition(out Vector3 worldPosition)
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame)
        {
            worldPosition = GetPointerPos();
            return true;
        }

        worldPosition = Vector3.zero;
        return false;
    }

    Vector3 GetPointerPos()
    {
        Vector2 mouseScreenPos = Pointer.current.position.ReadValue();
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

        mouseWorldPos.z = 0f;
        return mouseWorldPos;
    }
}
