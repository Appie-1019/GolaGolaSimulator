using System.Collections;
using UnityEngine;

public enum MovementType
{
    Default = 0,
    Inertia = 1,
    Smooth = 2,
    Rotate = 3,
    Reversal = 4,
    AppieSlide = 5,
    Seizure = 6,
    Statue = 7,
    Tracking = 8,
    NicePC = 9
}

public class MovementManager : MonoBehaviour
{
    [Header("Setter")]
    [SerializeField] private SwitchBox switchBox;
    [Header("BodyParts")]
    public PointerPos pointer;
    public GolaGolaBody body;
    public GolaGolaParts[] parts;
    public GameObject ThatBox;

    private MovementType currentType;
    private Vector3 seizureOffset;
    private Coroutine movementCoroutine;

    public static MovementManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (switchBox == null)
        {
            Debug.LogError(gameObject.name + " 은(는) 필요한 컴포넌트가 없음");
            this.enabled = false;
            return;
        }
        switchBox.AddToggleListener(SetMovementState);
    }

    private void Start()
    {
        ThatBox.SetActive(false);
        SetMovementState(MovementType.Default);
    }

    private void Update()
    {
        switch (currentType)
        {
            case MovementType.Default: Move_Default(); break;
            case MovementType.Inertia: Move_Inertia(); break;
            case MovementType.Smooth: Move_Smooth(); break;
            case MovementType.Rotate: Move_Rotate(); break;
            case MovementType.Reversal: Move_Reversal(); break;
            case MovementType.AppieSlide: Move_AppieSlide(); break;
            case MovementType.Seizure: Move_Seizure(); break;
            case MovementType.Statue: Move_Statue(); break;
            case MovementType.Tracking: Move_Tracking(); break;
            case MovementType.NicePC: Move_NicePC(); break;
            default: break;
        }
    }

    public void SetMovementState(MovementType type)
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }

        pointer.ResetAll();
        body.ResetAll();
        foreach (GolaGolaParts part in parts)
        {
            part.ResetAll();
        }

        currentType = type;

        if (currentType == MovementType.AppieSlide)
        {
            body.gameObject.SetActive(false);
            foreach (GolaGolaParts part in parts)
            {
                part.gameObject.SetActive(false);
            }
        }
        else
        {
            body.gameObject.SetActive(true);
            foreach (GolaGolaParts part in parts)
            {
                part.gameObject.SetActive(true);
            }
        }
        
        ThatBox.SetActive(false);

        switch (currentType)
        {
            case MovementType.Inertia:
                body.inertia = true;
                break;
            case MovementType.Seizure:
                movementCoroutine = StartCoroutine(SetSeizureOffset());
                break;
            case MovementType.NicePC:
                movementCoroutine = StartCoroutine(Move_NicePC());
                break;
            case MovementType.AppieSlide:
                ThatBox.SetActive(true);
                ThatBox.transform.position = body.originalPos;
                break;
        }
    }

    public void SetMovementState(int typeCode)
    {
        if (System.Enum.IsDefined(typeof(MovementType), typeCode))
        {
            MovementType type = (MovementType)typeCode;
            SetMovementState(type);
        }
        else
        {
            Debug.LogWarning($"{typeCode}는 유효한 MovementType이 아님");
        }
    }

    void Move_Default()
    {
        pointer.UpdatePosition();
        body.GotoPointer();

        foreach (GolaGolaParts part in parts)
        {
            part.LookAtBody();
        }
    }

    void Move_Inertia()
    {
        pointer.UpdatePosition();

        foreach (GolaGolaParts part in parts)
        {
            part.LookAtBody();
        }
    }

    void Move_Smooth()
    {
        pointer.UpdatePosition();

        body.transform.position = Vector2.Lerp(body.transform.position, pointer.transform.position, 5 * Time.deltaTime);

        foreach (GolaGolaParts part in parts)
        {
            part.LookAtBody();
        }
    }

    void Move_Rotate()
    {
        pointer.UpdatePosition();
        body.transform.Rotate(0, 0, -360.0f * Time.deltaTime * 1.5f);
        body.GotoPointer();

        foreach (GolaGolaParts part in parts)
        {
            part.LookAtBody();
        }
    }

    void Move_Reversal()
    {
        pointer.UpdatePosition();

        foreach (GolaGolaParts part in parts)
        {
            part.transform.position = pointer.transform.position + part.offset;
            part.LookAtBody();
        }
    }

    void Move_AppieSlide()
    {
        pointer.UpdatePosition();
    }

    void Move_Seizure()
    {
        pointer.UpdatePosition();
        body.transform.position = pointer.transform.position + seizureOffset;

        foreach (GolaGolaParts part in parts)
        {
            part.LookAtBody();
        }
    }

    IEnumerator SetSeizureOffset()
    {
        float interval = 0.1f;
        while (true)
        {
            seizureOffset = Random.insideUnitCircle * Random.value * 2;
            yield return new WaitForSeconds(interval);
        }
    }

    void Move_Statue()
    {
        pointer.UpdatePosition();
        body.GotoPointer();

        foreach (GolaGolaParts part in parts)
        {
            part.transform.position = pointer.transform.position + part.offset;
            part.LookAtBody();
        }
    }

    void Move_Tracking()
    {
        pointer.UpdatePosition();
        body.GotoPointer();

        foreach (GolaGolaParts part in parts)
        {
            part.transform.position = Vector2.Lerp(part.transform.position, pointer.transform.position + part.offset, 5 * Time.deltaTime);
            part.LookAtBody();
        }
    }

    IEnumerator Move_NicePC()
    {
        while (true)
        {
            pointer.UpdatePosition();
            body.GotoPointer();

            foreach (GolaGolaParts part in parts)
            {
                part.LookAtBody();
            }

            yield return new WaitForSeconds(Random.value / 2);
        }
    }
}
