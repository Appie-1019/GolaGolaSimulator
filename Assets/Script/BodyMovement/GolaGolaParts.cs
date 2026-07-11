using UnityEngine;

public class GolaGolaParts : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float angleOffset;
    
    [HideInInspector] public Vector3 offset;
    private Quaternion rotation;

    private void Awake()
    {
        if (target == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            return;
        }

        offset = transform.position;
        rotation = transform.rotation;
    }

    public void LookAtBody() => LookAtPos((Vector2)target.position);
    public void LookAtPos(Vector3 pos) => LookAtPos((Vector2)pos);
    public void LookAtPos(Vector2 pos)
    {
        Vector2 dir = pos - (Vector2)transform.position;
        float targetAngle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg + angleOffset;
        transform.rotation = Quaternion.Euler(0, 0, targetAngle);
    }

    public void ResetAll()
    {
        transform.position = offset;
        transform.rotation = rotation;
    }
}
