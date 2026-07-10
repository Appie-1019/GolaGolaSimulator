using System.Collections;
using UnityEngine;

public class GolaGolaBody : MonoBehaviour
{
    public Transform target;

    Coroutine spinCoroutine;
    Vector3 originalPos;

    private void Start()
    {
        originalPos = transform.position;
    }

    public void StartSpin()
    {
        StopSpin();
        spinCoroutine = StartCoroutine(Spin());
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

    //void Seizure()
    //{
    //    transform.position = target.position + (Vector3)seizureOffset;
    //}

    //IEnumerator SetSeizureOffset()
    //{
    //    float interval = 0.1f;
    //    while (true)
    //    {
    //        seizureOffset = Random.insideUnitCircle * Random.value;
    //        yield return new WaitForSeconds(interval);
    //    }
    //}

    public void ResetPosition()
    {
        StopSpin();
        transform.rotation = Quaternion.identity;
        transform.position = originalPos;
    }
}
