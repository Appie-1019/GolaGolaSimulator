using System;
using System.Collections;
using System.Globalization;
using UnityEngine;
using UnityEngine.Networking;

public class TimeManager : MonoBehaviour
{
    public static TimeManager Instance { get; private set; }

    public static float CustomDeltaTime;
    public static float CustomDeltaTimeFactor;

    public static DateTime CurrentTime { get; private set; }
    public static bool IsTimeSynced { get; private set; }

    private static readonly TimeSpan KST_OFFSET = TimeSpan.FromHours(9);
    private const string TIME_SERVER_URL = "https://www.google.com";
    private const float RESYNC_INTERVAL = 600f; // 재동기화 간격 (초)
    private const float RETRY_DELAY = 5f;   // 재시도 간격 (초)

    private float baseRealtime;
    private DateTime baseServerTimeKst;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        CustomDeltaTimeFactor = 1;
    }

    private void Start()
    {
        StartCoroutine(SyncRoutine());
    }

    private void Update()
    {
        CustomDeltaTime = Time.deltaTime * CustomDeltaTimeFactor;

        if (IsTimeSynced)
        {
            float elapsed = Time.realtimeSinceStartup - baseRealtime;
            CurrentTime = baseServerTimeKst.AddSeconds(elapsed);
        }
    }

    private IEnumerator SyncRoutine()
    {
        while (true)
        {
            yield return FetchServerTime();

            float waitTime = IsTimeSynced ? RESYNC_INTERVAL : RETRY_DELAY;
            yield return new WaitForSecondsRealtime(waitTime);
        }
    }

    private IEnumerator FetchServerTime()
    {
        using (UnityWebRequest request = UnityWebRequest.Head(TIME_SERVER_URL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                string dateHeader = request.GetResponseHeader("date");

                if (DateTime.TryParse(
                        dateHeader,
                        CultureInfo.InvariantCulture,
                        DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                        out DateTime serverUtcTime))
                {
                    baseServerTimeKst = serverUtcTime.Add(KST_OFFSET);
                    baseRealtime = Time.realtimeSinceStartup;

                    CurrentTime = baseServerTimeKst;
                    IsTimeSynced = true;
                }
                else
                {
                    Debug.LogError("TimeManager: 서버 시간 파싱 실패.");
                    IsTimeSynced = false;
                }
            }
            else
            {
                Debug.LogError("TimeManager: 서버 시간 요청 실패 - " + request.error);
                IsTimeSynced = false;
            }
        }
    }

    // 원래 자정부터 6시까지 "LEdge가 잘 시간"이라면 백도어 이용을 중단하려 했지만, 너무 끔찍한 생각이라 취소했습니다.
    //public static bool IsDawn(DateTime time)
    //{
    //    return time.Hour < 6;
    //}
}