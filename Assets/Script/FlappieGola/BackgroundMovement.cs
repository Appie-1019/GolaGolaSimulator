using UnityEngine;

public class BackgroundMovement : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("EndPos")]
    public float endXPosition = -20f;

    [Header("Sprite")]
    public int startSpriteCount = 5;
    public float spriteInterval = 10f;
    public float spriteScale = 3f;
    public bool randomSprite = false;
    public Sprite[] backgroundSprites;
    public int sortLayer = -100;
    [Header("Hitbox")]
    public bool isDeathZone = false;

    [HideInInspector] public bool canMove = false;
    private SpriteRenderer[] activeRenderers;
    private int frontIndex = 0;
    private int currentSpriteIndex = 0;

    private int lastRandomIndex = -1;

    private const float resetDistance = 3000f;

    void Start()
    {
        activeRenderers = new SpriteRenderer[startSpriteCount];
        StartSet();
    }

    void Update()
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

        SpriteRenderer frontRenderer = activeRenderers[frontIndex];

        if (frontRenderer.transform.position.x <= endXPosition)
        {
            int lastIndex = (frontIndex + startSpriteCount - 1) % startSpriteCount;
            SpriteRenderer lastRenderer = activeRenderers[lastIndex];

            Vector3 newLocalPos = frontRenderer.transform.localPosition;
            newLocalPos.x = lastRenderer.transform.localPosition.x + spriteInterval;
            frontRenderer.transform.localPosition = newLocalPos;

            if (randomSprite)
            {
                frontRenderer.sprite = backgroundSprites[GetNextRandomSpriteIndex()];
            }
            else
            {
                frontRenderer.sprite = backgroundSprites[currentSpriteIndex];
                currentSpriteIndex = (currentSpriteIndex + 1) % backgroundSprites.Length;
            }

            frontIndex = (frontIndex + 1) % startSpriteCount;
        }
    }

    void StartSet()
    {
        float currentLocalX = 0f;

        for (int i = 0; i < startSpriteCount; i++)
        {
            GameObject go = new GameObject("BG_" + i);
            go.transform.SetParent(transform);
            go.transform.localPosition = new Vector3(currentLocalX, 0f, 0f);
            go.transform.localScale = new Vector3(spriteScale, spriteScale, 1f);

            SpriteRenderer sr = go.AddComponent<SpriteRenderer>();
            sr.sortingOrder = sortLayer;

            if (randomSprite)
            {
                sr.sprite = backgroundSprites[GetNextRandomSpriteIndex()];
            }
            else
            {
                sr.sprite = backgroundSprites[currentSpriteIndex];
                currentSpriteIndex = (currentSpriteIndex + 1) % backgroundSprites.Length;
            }

            activeRenderers[i] = sr;

            currentLocalX += spriteInterval;

            if (isDeathZone)
            {
                BoxCollider2D col = go.AddComponent<BoxCollider2D>();
                col.isTrigger = true;
                go.tag = "Respawn";
            }
        }
    }

    private int GetNextRandomSpriteIndex()
    {
        if (backgroundSprites.Length <= 1) return 0;

        int newIndex = Random.Range(0, backgroundSprites.Length);

        while (newIndex == lastRandomIndex)
        {
            newIndex = Random.Range(0, backgroundSprites.Length);
        }

        lastRandomIndex = newIndex;
        return newIndex;
    }
}