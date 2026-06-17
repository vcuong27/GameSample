using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
