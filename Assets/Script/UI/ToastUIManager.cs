using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary> 토스트 메시지의 동작 데이터 구조체 </summary>
[Serializable]
public class ToastUIData
{
    [Header("Time, Default Position Set")]
    public Vector2 StartAnchoredPosition;   // 시작 위치
    public Vector2 EndAnchoredPosition; // 첫 번째 표시 위치
    public float MoveDuration;  // 움직임 기간
    public float ShowTime;  // 표시 기간 (이후 파괴)

    [Header("Displacement")]
    public Vector2 FirstDisplacementSizeDelta;  // 첫 번째 쌓임에서 바뀔 SizeDelta
    public Vector2 FirstDisplacementOffset; // 첫 번째 쌓임에서 이동할 벡터
    public Vector2 DisplacementOffset;  // 첫 번째가 아닌 쌓임에서 이동할 벡터
    public Vector2 DestroyOffset;   // 파괴될 때 움직일 벡터

    [Header("Displacement Text")]
    public int DisplacementCharactersNumber;    // 쌓임이 발생할 경우 최대 글자 제한
    public float DisplacementTextFontSize;  // 쌓임이 발생할 경우 글자의 폰트 크기

    public int MaxAccumulation; // 최대 쌓임 횟수 제한
}

public class ToastUIManager : MonoBehaviour
{
    public static ToastUIManager Instance { get; private set; }

    [Header("DATA")]
    public ToastUIData data;
    [Header("UI Object")]
    public ToastUI toastMessege;
    public Transform canvas;

    private List<ToastUI> toastUIs = new List<ToastUI>();
    private readonly Color DefaultTextColor = Color.white;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        StartCoroutine(FirstToast());
    }

    /// <summary>
    /// 프로그램 시작 시 첫 번째 토스트 메시지 출력
    /// </summary>
    IEnumerator FirstToast()
    {
        yield return new WaitForSeconds(4f);
        if (!DataManager.isMobile)
        {
            AddToast("마우스를 움직여 GolaGola", Color.yellow);
            yield return new WaitForSeconds(3f);
            AddToast("[M] : 눌러서 마우스 숨기기 토글", Color.yellow);
            yield return new WaitForSeconds(3f);
            AddToast("[TAB] : 눌러서 메뉴 토글", Color.yellow);
        }
        else
        {
            AddToast("화면을 드래그하여 GolaGola");
            yield return new WaitForSeconds(3f);
            AddToast("두 손가락으로 꾹 눌러 메뉴 토글");
        }
    }

    /// <summary> <paramref name="messege"/>를 글자로 하는 토스트 메시지 출력 </summary>
    /// <param name="messege">토스트 메시지의 글자</param>
    public void AddToast(string messege)
    {
        AddToast(messege, DefaultTextColor);
    }

    public void AddToast(string messege, Color textColor)
    {
        if (!DataManager.CanToast) return;

        StackAllToast();
        ToastUI newMessegeUI = Instantiate(toastMessege, canvas);
        newMessegeUI.SetData(data);
        newMessegeUI.SetTextColor(textColor);
        newMessegeUI.Show(messege);
        toastUIs.Add(newMessegeUI);
    }

    /// <summary> 모든 존재하는 <see cref="ToastUI"/> 의 쌓음 처리 (<see cref="StackAllToast"/>) 호출 </summary>
    public void StackAllToast()
    {
        foreach (var t in toastUIs)
        {
            t.Stack();
        }
    }
}