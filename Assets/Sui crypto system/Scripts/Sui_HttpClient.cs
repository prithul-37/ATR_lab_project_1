using System;
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Sui
{
    [Serializable]
    public class TransactionRequest
    {
        public string command;
        public string userId;
        public string parameters;
    }

    [Serializable]
    public class TransactionResponse
    {
        public bool success;
        public string message;
        public string transactionId;
        public string data;
    }

    [Serializable]
    public class StatusUpdate
    {
        public string status;
        public string message;
        public string transactionId;
    }

    public class Sui_HttpClient : MonoBehaviour
    {
        [Header("Server Configuration")]
        [SerializeField] string _serverUrl = "http://localhost:8080";
        [SerializeField] string _transactionEndpoint = "/transaction";
        [SerializeField] string _statusEndpoint = "/status";

        [Header("Settings")]
        [SerializeField] int _requestTimeout = 30;

        public static Sui_HttpClient Instance;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        /// <summary>
        /// Initiates a transaction by sending a POST request to the server
        /// </summary>
        public void InitiateTransaction(string command, string userId, string parameters, Action<bool, TransactionResponse> callback)
        {
            TransactionRequest request = new TransactionRequest
            {
                command = command,
                userId = userId,
                parameters = parameters
            };

            StartCoroutine(PostTransaction(request, callback));
        }

        /// <summary>
        /// Posts transaction data to the server
        /// </summary>
        private IEnumerator PostTransaction(TransactionRequest request, Action<bool, TransactionResponse> callback)
        {
            string jsonData = JsonUtility.ToJson(request);
            string url = _serverUrl + _transactionEndpoint;

            Debug.Log($"[Sui_HttpClient] Posting transaction to {url}: {jsonData}");

            using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
            {
                byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
                webRequest.uploadHandler = new UploadHandlerRaw(bodyRaw);
                webRequest.downloadHandler = new DownloadHandlerBuffer();
                webRequest.SetRequestHeader("Content-Type", "application/json");
                webRequest.timeout = _requestTimeout;

                yield return webRequest.SendWebRequest();

                if (webRequest.result == UnityWebRequest.Result.Success)
                {
                    string responseText = webRequest.downloadHandler.text;
                    Debug.Log($"[Sui_HttpClient] Transaction response: {responseText}");

                    TransactionResponse response = JsonUtility.FromJson<TransactionResponse>(responseText);
                    callback?.Invoke(true, response);

                    // Show status in UI
                    if (Sui_LevelManager.Instance != null)
                    {
                        Status status = response.success ? Status.Success : Status.Failed;
                        Sui_LevelManager.Instance.ShowStatus(status, response.message);
                    }
                }
                else
                {
                    Debug.LogError($"[Sui_HttpClient] Transaction failed: {webRequest.error}");

                    TransactionResponse errorResponse = new TransactionResponse
                    {
                        success = false,
                        message = $"Request failed: {webRequest.error}",
                        transactionId = "",
                        data = ""
                    };

                    callback?.Invoke(false, errorResponse);

                    // Show error in UI
                    if (Sui_LevelManager.Instance != null)
                    {
                        Sui_LevelManager.Instance.ShowStatus(Status.Failed, errorResponse.message);
                    }
                }
            }
        }
    }
}
