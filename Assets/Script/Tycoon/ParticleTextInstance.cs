using System.Collections;
using TMPro;
using UnityEngine;

public class ParticleTextInstance : MonoBehaviour
{
    [Header("Compo")]
    [SerializeField] private TextMeshPro text;
    [SerializeField] private Rigidbody2D rb;
    [Header("Movement")]
    [SerializeField] private float actTime;
    [SerializeField] private float maxRotationSpeed;
    [SerializeField] private float minFireSpeed;
    [SerializeField] private float maxFireSpeed;
    [Header("Color")]
    [SerializeField] private Color startColor;
    [SerializeField] private Color endColor;

    [HideInInspector] public Transform initPos;

    private Coroutine ColorTransitionCoroutine;

    public void Init()
    {
        text.text = $"+{TycoonGameManager.Instance.incrementValue} GJ";

        transform.position = initPos.position;
        transform.rotation = Quaternion.identity;

        rb.angularVelocity = Random.Range(0, maxRotationSpeed);
        Vector2 randomVelocity = Random.onUnitCircle;
        randomVelocity.y = Mathf.Abs(randomVelocity.y);
        rb.linearVelocity = randomVelocity * Random.Range(minFireSpeed, maxFireSpeed);

        if (ColorTransitionCoroutine != null) StopCoroutine(ColorTransitionCoroutine);
        ColorTransitionCoroutine = StartCoroutine(ColorTransition());
    }

    IEnumerator ColorTransition()
    {
        float elapsedTime = 0;
        while (elapsedTime < actTime)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / actTime;
            text.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }

        ColorTransitionCoroutine = null;
        ParticleTextManager.Instance.BackToPool(this);
    }
}
