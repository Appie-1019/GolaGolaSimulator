using DG.Tweening;
using UnityEngine;
using static UnityEngine.Audio.ProcessorInstance;

public class FlappieGolaPipe : MonoBehaviour
{
    public SpriteRenderer up;
    public SpriteRenderer down;
    public Sprite evilPipeSprite;
    public Sprite goodPipeSprite;
    public float pipeMoveScale;
    public float pipeMoveDuration;

    private Transform upPipe;
    private Transform downPipe;
    private Vector3 ogUpPipePos;
    private Vector3 ogDownPipePos;
    private Vector3 moveUpPipePos;
    private Vector3 moveDownPipePos;

    private void Start()
    {
        ReRollSprite();

        upPipe = up.GetComponent<Transform>();
        downPipe = down.GetComponent<Transform>();
        ogUpPipePos = upPipe.localPosition;
        moveUpPipePos = ogUpPipePos + (Vector3.up * pipeMoveScale);
        ogDownPipePos = downPipe.localPosition;
        moveDownPipePos = ogUpPipePos + (Vector3.down * pipeMoveScale);

    }

    public void Init()
    {
        upPipe.localPosition = ogUpPipePos;
        downPipe.localPosition = ogDownPipePos;
        ReRollSprite();
    }

    public void ReRollSprite()
    {
        up.sprite = IsLucky() ? goodPipeSprite : evilPipeSprite;
        down.sprite = IsLucky() ? goodPipeSprite : evilPipeSprite;
        // 삼항연산 으히히히ㅣㅎㅎ
    }

    private bool IsLucky()
    {
        return Random.value <= 0.001f;
        //return Random.value <= 0.5f;
    }

    public void MovePipe()
    {
        upPipe.DOLocalMove(moveUpPipePos, pipeMoveDuration).SetEase(Ease.InQuart);
        downPipe.DOLocalMove(moveDownPipePos, pipeMoveDuration).SetEase(Ease.InQuart);
    }
}
