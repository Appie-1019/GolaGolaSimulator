using System.Collections;
using UnityEngine;

public class GolaGolaBody : MonoBehaviour
{
    public Transform target;

    Vector2 seizureOffset;

    private void Start()
    {
        //StartCoroutine(SetSeizureOffset());
    }


    private void LateUpdate()
    {
        DefaultMovement();
        //Seizure();
    }

    void DefaultMovement()
    {
        transform.position = target.position;
    }

    void Seizure()
    {
        transform.position = target.position + (Vector3)seizureOffset;
    }

    IEnumerator SetSeizureOffset()
    {
        float interval = 0.1f;
        while (true)
        {
            seizureOffset = Random.insideUnitCircle * Random.value;
            yield return new WaitForSeconds(interval);
        }
    }
}
