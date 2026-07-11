using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class GolaGolaBody : MonoBehaviour
{
    public Transform pointer;

    [HideInInspector] public bool inertia = false;

    Coroutine spinCoroutine;
    [HideInInspector] public Vector3 originalPos;
    Quaternion originalRotation;
    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            this.enabled = false;
            return;
        }

        originalPos = transform.position;
        originalRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        if (inertia) Move_Inertia();
    }

    public void StartSpin()
    {
        StopSpin();
        spinCoroutine = StartCoroutine(Spin());
    }

    public void GotoPointer()
    {
        transform.position = pointer.position;
    }

    public void StopSpin()
    {
        if (spinCoroutine == null) return;
        StopCoroutine(spinCoroutine);
        spinCoroutine = null;
    }

    IEnumerator Spin()
    {
        while (true)
        {
            transform.Rotate(0, 0, -340 * Time.deltaTime);
            yield return null;
        }
    }

    private void Move_Inertia()
    {
        if (pointer == null) return;

        Vector3 springForce = (pointer.position - transform.position) * 200;
        Vector3 dampingForce = rb.linearVelocity * 5;
        Vector3 finalForce = springForce - dampingForce;

        rb.AddForce(finalForce);
    }

    public void ResetAll()
    {
        inertia = false;
        rb.linearVelocity = Vector2.zero;
        StopSpin();

        transform.rotation = originalRotation;
        transform.position = originalPos;
    }
}
