using TMPro;
using UnityEngine;

public class ChatBoxUI : MonoBehaviour
{

    [SerializeField]
    TMP_InputField input;


    public void AddMessage()
    {
        ChatManager.Instance.SendChatMessage(input.text);
        input.text = "";
    }
}
