using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace MyGame.PlayTesters
{
    public class TesterLoginMenu : MonoBehaviour
    {
        [SerializeField]private PlaytimeReporter _playtimeReporter;
        [SerializeField]private TMP_InputField _accessKeyInput;
        [SerializeField]private TMP_Text _statusText;
        [SerializeField]private float _loginCooldownSeconds = 30f;
        [SerializeField]private int _maxFailedAttempts = 4;
        #if UNITY_EDITOR || DEVELOPMENT_BUILD
        private const string BaseUrl = "http://localhost:5183/api";
        #else
        private const string BaseUrl = "https://my-playtestersapi-prod.up.railway.app/api";
        #endif
        private bool _isLoggingIn;
        private int _failedAttemptCount;

        [Serializable]
        private class LoginRequest
        {
            public string AccessKey;
        }

        [Serializable]
        private class LoginResponse
        {
            public bool success;
            public string status;
            public string message;
        }

        public void OnLoginClicked()
        {
            if (_isLoggingIn)
                return;

            var accessKey = _accessKeyInput.text.Trim();
            if (string.IsNullOrWhiteSpace(accessKey))
            {
                _statusText.text = "Please enter your key.";
                return;
            }

            StartCoroutine(LoginCoroutine(accessKey));
        }

        private IEnumerator LoginCoroutine(string accessKey)
        {
            _isLoggingIn = true;
            _statusText.text = "Validating...";

            var request = new LoginRequest
            {
                AccessKey = accessKey
            };

            var json = JsonUtility.ToJson(request);
            var apiUrl = $"{BaseUrl}/testers/validate-access";
            using var www = new UnityWebRequest(apiUrl, "POST");
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
            www.uploadHandler = new UploadHandlerRaw(bodyRaw);
            www.downloadHandler = new DownloadHandlerBuffer();
            www.SetRequestHeader("Content-Type", "application/json");

            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.ConnectionError)
            {
                _statusText.text = $"{www.error}.";
                _isLoggingIn = false;
                yield break;
            }

            var response = JsonUtility.FromJson<LoginResponse>(www.downloadHandler.text);
            if (!response.success)
            {
                var errorMessage = response.status switch
                {
                    "Invalid" => "Invalid access key. Please try again.",
                    "CriticalError" => "Server error. Please try again later.",
                    _ => "Unexpected error. Please contact the dev team."
                };
                _statusText.text = errorMessage;
                _isLoggingIn = false;
                _failedAttemptCount++;

                if (_failedAttemptCount == _maxFailedAttempts)
                {
                    _statusText.text = "Too many attempts.\n " +
                        $"Try again in {_loginCooldownSeconds} seconds.";
                    _isLoggingIn = true;
                    yield return new WaitForSeconds(_loginCooldownSeconds);
                    _failedAttemptCount = 0;
                    _isLoggingIn = false;
                }

                yield break;
            }

            _playtimeReporter.StartReporting(request.AccessKey);
            SceneManager.LoadScene("MainMenu");
        }
    }
}
