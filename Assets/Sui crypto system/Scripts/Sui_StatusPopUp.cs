using TMPro;
using UnityEngine;

public class Sui_StatusPopUp : MonoBehaviour
{
    [SerializeField]TextMeshProUGUI _messageText;
    public void PopUp(string message)
    {
        _messageText.text = message;
    }
}
