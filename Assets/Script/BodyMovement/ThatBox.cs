using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class ThatBox : MonoBehaviour
{
    public GameObject pointer;

    public Rigidbody2D rb;

    private void Update()
    {
        transform.Rotate(0, 360.0f * Time.deltaTime * 1.2f, 0);

        if (transform.position.magnitude > 20.0f)
        {
            transform.position = Vector3.ClampMagnitude(transform.position, 20f);
            rb.linearVelocity = Vector2.zero;
        }
    }

    private void FixedUpdate()
    {
        if (pointer == null) return;

        Vector3 springForce = (pointer.transform.position - transform.position) * 1.5f;
        if (springForce.magnitude <= 4) springForce = springForce.normalized * 4;

        rb.AddForce(springForce);
    }
}
