using System;
using UnityEngine;

namespace Sui
{
    public class Sui_LevelManager : MonoBehaviour
    {

        [SerializeField] GameObject _popUpPanel;
        [SerializeField] Sui_ConfirmationPopUp _sui_Pop;
        [SerializeField] Sui_StatusPopUp _sui_StatusPopUp;

        public static Sui_LevelManager Instance;

        private void Awake()
        {
            Instance = this;
        }

        public bool _isProcessing = false;
        public void Robot1Function(Action callback)
        {
            if (_isProcessing) return;
            _isProcessing = true;
            _popUpPanel.SetActive(true);
            _sui_Pop.PopUp((promt) =>
            {
                // Send transaction to server
                if (Sui_HttpClient.Instance != null)
                {
                    Sui_HttpClient.Instance.InitiateTransaction(
                        command: promt,
                        userId: "Robot1",
                        parameters: "",
                        callback: (success, response) =>
                        {
                            Debug.Log($"Robot1 Transaction: {response.message}");

                        }
                    );
                }
                else
                {
                    Debug.Log(promt);

                }
                _isProcessing = false;
                _popUpPanel.SetActive(false);
                callback?.Invoke();
            });

        }

        public void Robot2Function(Action callback)
        {
            if (_isProcessing) return;
            _isProcessing = true;
            _popUpPanel.SetActive(true);
            _sui_Pop.PopUp((promt) =>
            {
                // Send transaction to server
                if (Sui_HttpClient.Instance != null)
                {
                    Sui_HttpClient.Instance.InitiateTransaction(
                        command: promt,
                        userId: "Robot2",
                        parameters: "",
                        callback: (success, response) =>
                        {
                            Debug.Log($"Robot2 Transaction: {response.message}");
                        }
                    );
                }
                else
                {
                    Debug.Log(promt);

                }
                _isProcessing = false;
                _popUpPanel.SetActive(false);
                callback?.Invoke();
            });

        }


        public void Robot3Function(Action callback)
        {
            if (_isProcessing) return;
            _isProcessing = true;
            _popUpPanel.SetActive(true);
            _sui_Pop.PopUp((promt) =>
            {
                // Send transaction to server
                if (Sui_HttpClient.Instance != null)
                {
                    Sui_HttpClient.Instance.InitiateTransaction(
                        command: promt,
                        userId: "Robot3",
                        parameters: "",
                        callback: (success, response) =>
                        {
                            Debug.Log($"Robot3 Transaction: {response.message}");

                        }
                    );
                }
                else
                {
                    Debug.Log(promt);
                }

                _isProcessing = false;
                _popUpPanel.SetActive(false);
                callback?.Invoke();
            });

        }

        public void ShowStatus(Status status, string message)
        {
            if (status == Status.Success)
            {
                Debug.Log(message);
            }
            if (status == Status.Failed)
            {
                Debug.LogError(message);
            }

            _sui_StatusPopUp.PopUp(message);
            _sui_StatusPopUp.gameObject.SetActive(true);

        }


        public void CancelPopUp()
        {
            _isProcessing = false;
        }

    }

    public enum Status
    {
        Success = 0,
        Failed = 1
    }
}

