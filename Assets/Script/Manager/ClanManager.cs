using System;
using UnityEngine;

public enum ClanStatus
{
    NONE,
    MEMBER,
    LEADER
}

public class ClanManager : MonoBehaviour
{

    public static Action OnCLanCreated;
    public static Action OnCLanInfoReceived;


    private static ClanManager _instance;
    public static ClanManager Instance
    {
        get
        {
            return _instance;
        }
    }

    private bool isInitialized = false;

    private int clanID;
    private string playerOwner;
    private string name;
    private int score;
    private ClanStatus status;
    private ClanInfo clanInfo;

    private void Awake()
    {
        if (_instance != null)
            return;

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void Initlize()
    {
        isInitialized = true;
    }

    public bool IsInitialized()
    {
        return isInitialized;
    }

    public void CreateClan(string clanName)
    {
        string jsondata = JsonUtility.ToJson(
            new ClanInfo()
            {
                memberCount = 1,
                message = "Welcome to our clan!",
                flagName = "default_flag",
                listPlayerRequestID = new int[0]

            });

        OnlineManager.Instance.CreateClan(clanName, jsondata);

    }

    public void OnClanCreated(SC_ClanCreate clanCreateMessage)
    {
        OnCLanCreated?.Invoke();
        GetClanInfo(clanCreateMessage.clanID);
    }

    public void GetClanInfo(int clanID)
    {
        OnlineManager.Instance.GetClanInfo(clanID);
    }

    public void OnClanInfoReceived(SC_ClanInfo clanInfoMessage)
    {
        if(clanInfoMessage.getInfoResult == MessageStatus.SUCCESS)
        {
            clanID = clanInfoMessage.clanID;
            playerOwner = clanInfoMessage.playerOwner;
            name = clanInfoMessage.name;
            score = clanInfoMessage.score;
            status = clanInfoMessage.ownerID == PlayerProfile.Instance.GetPlayerID() ? ClanStatus.LEADER : ClanStatus.MEMBER;
            clanInfo = JsonUtility.FromJson<ClanInfo>(clanInfoMessage.jsonData);
        }

        OnCLanInfoReceived?.Invoke();
    }



}
