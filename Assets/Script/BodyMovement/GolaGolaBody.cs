using UnityEngine;

public class GolaGolaBody : MonoBehaviour
{
    public Transform target;

    private void LateUpdate()
    {
        transform.position = target.position;
    }
}
