using System;
using UnityEngine;

public class ChatManager : Singleton<ChatManager>
{

    public static Action OnChatMessageReceived;
    public static Action OnChatHistoriesReceived;

    private bool isInitialized = false;

    public void Initialize()
    {
        isInitialized = true;
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public void GetChatHistories()
    {
        OnlineManager.Instance.GetChatHistory();
    }

    public void SendChatMessage(string message)
    {
        OnlineManager.Instance.SendChatMessage(message);
    }

    public void ReceiveChatMessage(SC_ChatMessage message)
    {
        Debug.Log("Received chat message: " + message.messages);
    }

    public void ReceiveChatHistories(SC_ChatHistories histories)
    {
        Debug.Log("Received chat histories: " + histories.messages.Count + " messages.");
    }

}
