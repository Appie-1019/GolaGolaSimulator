using UnityEngine;

public class GolaGolaParts : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private Vector2 targetOffset;
    [SerializeField] private float angleOffset;

    void Update()
    {
        if (target != null)
        {
            LookAtPos((Vector2)target.position + targetOffset);
        }
    }

    private void LookAtPos(Vector2 pos)
    {
        Vector2 dir = pos - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }
}
