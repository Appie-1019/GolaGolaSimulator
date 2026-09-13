using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class FlappieGolaPlayer : MonoBehaviour
{
    [Header("Jump Settings")]
    public float jumpPower = 7f;
    public float minVelocity = -7f;

    [Header("Rotation Settings")]
    public float maxRotationAngle = 35f;
    public float minRotationAngle = -90f;
    [Header("Sound")]
    public AudioClip[] JumpSounds;

    [HideInInspector] public Rigidbody2D rb;
    private Vector2 jumpVector;
    private bool fixedPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        jumpVector = new Vector2(0, jumpPower);
    }

    private void Update()
    {
        if (fixedPos || !FlappieGolaManager.GameOn) return;

        if (CheckJumpInput())
        {
            AudioManager.Instance.PlayRandom2DSound(JumpSounds, SoundType.Game);
            rb.linearVelocity = jumpVector;
        }

        float t = Mathf.InverseLerp(minVelocity, jumpPower, rb.linearVelocity.y);
        float currentAngle = Mathf.Lerp(minRotationAngle, maxRotationAngle, t);
        transform.rotation = Quaternion.Euler(0, 0, currentAngle);

        if (transform.position.y >= 11)
        {
            if (FlappieGolaManager.Instance == null) return;
            FlappieGolaManager.Instance.GameOver();
        }
    }

    private bool CheckJumpInput()
    {
        if (Pointer.current != null && Pointer.current.press.wasPressedThisFrame) return true;
        else if (Keyboard.current != null)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame) return true;
        }

        return false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!FlappieGolaManager.GameOn) return;

        if (collision.CompareTag("Respawn"))
        {
            if (FlappieGolaManager.Instance == null) return;
            FlappieGolaManager.Instance.GameOver();
        }
    }

    public void FixPosition()
    {
        StopAllCoroutines();
        StartCoroutine(WaitForRelease());
    }

    IEnumerator WaitForRelease()
    {
        fixedPos = true;

        Vector3 position = transform.position;
        position.y = 0;
        transform.position = position;
        transform.rotation = Quaternion.identity;
        rb.simulated = false;

        yield return new WaitUntil(() => FlappieGolaManager.GameOn);
        yield return new WaitUntil(CheckJumpInput);

        rb.simulated = true;
        rb.linearVelocity = jumpVector;
        AudioManager.Instance.PlayRandom2DSound(JumpSounds, SoundType.Game);
        fixedPos = false;
    }
}
