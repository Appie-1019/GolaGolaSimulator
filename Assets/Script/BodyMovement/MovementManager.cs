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

    private MovementType currentType;
    
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
        pointer.ResetAll();
        body.ResetAll();
        foreach (GolaGolaParts part in parts)
        {
            part.ResetAll();
        }

        currentType = type;

        switch (currentType)
        {
            case MovementType.Inertia:
                body.inertia = true;
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

    }

    void Move_Seizure()
    {

    }

    void Move_Statue()
    {

    }

    void Move_Tracking()
    {

    }

    void Move_NicePC()
    {

    }
}
