using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Sui
{
    public class Sui_ConfirmationPopUp : MonoBehaviour
    {
        [SerializeField] Button _confirmPromtButton;
        [SerializeField] Button _confirmTransactionButton;
        [SerializeField] TMP_InputField _commandText;

        [SerializeField] GameObject _confirmPromtPopUI;
        [SerializeField] GameObject _confirmTransactionPopUI;

        string _command;

        private void OnEnable()
        {
            _confirmPromtButton.onClick.RemoveAllListeners();
            _confirmPromtButton.onClick.AddListener(() =>
            {
                _command = _commandText.text;

                _confirmPromtPopUI.SetActive(false);
                _confirmTransactionPopUI.SetActive(true);
            });

        }

        public void PopUp(Action<string> callback = null)
        {
            _commandText.text = "";
            _confirmPromtPopUI.SetActive(true);
            _confirmTransactionPopUI.SetActive(false);

            _confirmTransactionButton.onClick.RemoveAllListeners();
            _confirmTransactionButton.onClick.AddListener(() => callback?.Invoke(_command));
        }
    }
}

