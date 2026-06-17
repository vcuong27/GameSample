using System;
using System.Dynamic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class ChatManager : MonoBehaviour
{

    private static ChatManager _instance;
    public static ChatManager Instance
    {
        get
        {
            return _instance;
        }
    }

    public static Action OnChatMessageReceived;
    public static Action OnChatHistoriesReceived;

    private bool isInitialized = false;


    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

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
