using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

namespace MyGame.PlayTesters
{
    public class PlaytimeReporter : MonoBehaviour
    {
        [SerializeField]private float _reportIntervalSeconds = 120f;
        private Coroutine _playtimeCoroutine;
        private DateTime _lastReportTime;
        private string _accessKey;
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        private const string BaseUrl = "http://localhost:5183/api";
        #else
        private const string BaseUrl = "https://my-playtestersapi-prod.up.railway.app/api";
        #endif
        private static PlaytimeReporter s_instance;

        [Serializable]
        private class UpdatePlaytimeRequest
        {
            public double HoursPlayed;
        }

        private void Awake()
        {
            if (s_instance != null && s_instance != this)
            {
                Destroy(gameObject);
                return;
            }

            s_instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void StartReporting(string accessKey)
        {
            _accessKey = accessKey;
            _lastReportTime = DateTime.UtcNow;
            if (_playtimeCoroutine != null)
            {
                StopCoroutine(_playtimeCoroutine);
            }
            _playtimeCoroutine = StartCoroutine(ReportCoroutine());
        }

        private IEnumerator ReportCoroutine()
        {
            while (true)
            {
                yield return new WaitForSecondsRealtime(_reportIntervalSeconds);
                double hoursPlayed = (DateTime.UtcNow - _lastReportTime).TotalHours;
                _lastReportTime = DateTime.UtcNow;

                var request = new UpdatePlaytimeRequest 
                { 
                    HoursPlayed = hoursPlayed 
                };

                var json = JsonUtility.ToJson(request);
                var apiUrl = $"{BaseUrl}/testers/{_accessKey}/playtime";
                Debug.Log($"[PlaytimeReporter] Sending playtime update: {hoursPlayed}h");

                using var www = new UnityWebRequest(apiUrl, "PATCH");
                byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"Playtime update failed: {www.error}");
                }
            }
        }
    }
}
