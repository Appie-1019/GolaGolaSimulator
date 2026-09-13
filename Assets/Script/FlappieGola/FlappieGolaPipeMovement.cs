using UnityEngine;

public class FlappieGolaPipeMovement : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;
    [Header("EndPos")]
    public float endXPosition = -20f;
    [Header("Pipe")]
    public int startPipeCount = 5;
    public float pipeInterval = 10f;
    public FlappieGolaPipe pipePrefab;
    [Header("PipeYPosition")]
    public float maxPipeInterval;
    public float maxY;
    public float minY;
    [Header("PointAddPosition")]
    public float playerXPosition;

    [HideInInspector] public bool canMove = false;
    private const float resetDistance = 3000f;
    private FlappieGolaPipe[] activePipes;
    private float lastPipeYPos;
    private int frontIndex = 0;
    private int playerFrontIndex = 0;
    private bool completedStartSet = false;
    private Vector3 ogPos;

    private void Awake()
    {
        ogPos = transform.position;
    }

    void StartSet()
    {
        activePipes = new FlappieGolaPipe[startPipeCount];
        lastPipeYPos = 0;
        float currentLocalX = 0f;

        for (int i = 0; i < startPipeCount; i++)
        {
            FlappieGolaPipe newPipe = Instantiate(pipePrefab);
            newPipe.transform.SetParent(transform);
            newPipe.transform.localPosition = i == 0 ? new Vector3(currentLocalX, 0f, 0f) : new Vector3(currentLocalX, GetNextPipeYPos(), 0f);
            // 아!!!! 삼항연산 先輩♥♥♥♥!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! 슼@게!!!!!!!!!!!!!!!!!!

            currentLocalX += pipeInterval;
            activePipes[i] = newPipe;
        }

        completedStartSet = true;
    }

    float GetNextPipeYPos()
    {
        float randomY = Random.Range(minY, maxY);
        float clampY = Mathf.Clamp(randomY, lastPipeYPos - maxPipeInterval, lastPipeYPos + maxPipeInterval);
        lastPipeYPos = clampY;
        return clampY;
    }

    private void Update()
    {
        if (!canMove) return;

        Vector2 move = new Vector2(-moveSpeed * Time.deltaTime, 0f);
        transform.position += (Vector3)move;

        if (transform.position.x <= -resetDistance)
        {
            float overshoot = transform.position.x;
            transform.position = new Vector3(0, transform.position.y, transform.position.z);

            foreach (Transform child in transform)
            {
                child.localPosition += new Vector3(overshoot, 0, 0);
            }
        }

        FlappieGolaPipe frontPipe = activePipes[frontIndex];

        if (frontPipe.transform.position.x <= endXPosition)
        {
            frontPipe.Init();
            int lastIndex = (frontIndex + startPipeCount - 1) % startPipeCount;
            FlappieGolaPipe lastPipe = activePipes[lastIndex];

            Vector3 newLocalPos = frontPipe.transform.localPosition;
            newLocalPos.x = lastPipe.transform.localPosition.x + pipeInterval;
            newLocalPos.y = GetNextPipeYPos();
            frontPipe.transform.localPosition = newLocalPos;

            frontIndex = (frontIndex + 1) % startPipeCount;
        }

        FlappieGolaPipe playerFrontPipe = activePipes[playerFrontIndex];
        if (playerXPosition >= playerFrontPipe.transform.position.x)
        {
            FlappieGolaManager.Instance.AddScore();
            playerFrontPipe.MovePipe();
            playerFrontIndex = (playerFrontIndex + 1) % startPipeCount;
        }
    }

    public void Init()
    {
        if (!completedStartSet) StartSet();
        else
        {
            transform.position = ogPos;
            canMove = false;
            lastPipeYPos = 0;
            frontIndex = playerFrontIndex = 0;

            float currentLocalX = 0f;
            for (int i = 0; i < startPipeCount; i++)
            {
                if (activePipes[i] != null)
                {
                    activePipes[i].transform.localPosition = i == 0 ? new Vector3(currentLocalX, 0f, 0f) : new Vector3(currentLocalX, GetNextPipeYPos(), 0f);
                    activePipes[i].Init();
                    currentLocalX += pipeInterval;
                }
            }
        }
    }
}
