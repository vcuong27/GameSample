using System;
using System.Collections.Generic;
using UnityEngine;

public enum ClanStatus
{
    NONE,
    MEMBER,
    LEADER
}

public class ClanManager : Singleton<ClanManager>
{

    public static Action OnCLanCreated;
    public static Action OnCLanInfoReceived;
    public static Action OnCLanListReceived;
    public static Action OnClanWarStarted;
    public static Action OnClanWarInfoReceived;

    private bool isInitialized = false;
    private bool isReceivedClanInfo = false;

    private int clanID;
    private string playerOwner;
    private string name;
    private int score;
    private ClanStatus status;
    private ClanInfo clanInfo;
    private List<ClanListInfo> clanListInfo;
    private int currentWarID;
    private SC_ClanWarInfo currentWarInfo;

    public void Initlize()
    {
        isInitialized = true;
        isReceivedClanInfo = false;
        clanListInfo = null;
        clanInfo = null;
        status = ClanStatus.NONE;
        score = 0;

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

    public string GetClanName()
    {
        return name;
    }

    public string GetPlayerOwner()
    {
        return playerOwner;
    }

    public ClanInfo GetClanInfo()
    {
        if (isReceivedClanInfo)
        {
            return clanInfo;
        }
        else
        {
            Debug.LogWarning("Clan information has not been received yet.");
            return null;
        }
    }

    public void OnClanInfoReceived(SC_ClanInfo clanInfoMessage)
    {
        clanID = clanInfoMessage.clanID;
        playerOwner = clanInfoMessage.playerOwner;
        name = clanInfoMessage.name;
        score = clanInfoMessage.score;
        status = clanInfoMessage.ownerID == PlayerProfile.Instance.GetPlayerID() ? ClanStatus.LEADER : ClanStatus.MEMBER;
        clanInfo = JsonUtility.FromJson<ClanInfo>(clanInfoMessage.jsonData);
        PlayerProfile.Instance.SetClanID(clanID);
        isReceivedClanInfo = true;
        OnCLanInfoReceived?.Invoke();
    }

    public bool IsClanInfoReceived()
    {
        return isReceivedClanInfo;
    }

    public void GetListClan(int index)
    {
        OnlineManager.Instance.GetListClan(index);
    }

    public void OnListClanReceived(List<ClanListInfo> listClan)
    {
        clanListInfo = listClan;
    }

    public List<ClanListInfo> GetClanListInfo()
    {
        return clanListInfo;
    }

    public void AttackClanID(int OtherClanID)
    {
        OnlineManager.Instance.AttackClan(clanID, OtherClanID);
    }

    public void ClanWarStarted(SC_ClanWarStart warStartMessage)
    {
        currentWarID = warStartMessage.warID;
    }


    public void GetClanWarInfo()
    {
        OnlineManager.Instance.GetClanWarInfo(currentWarID);
    }

    public void ClanWarInfoReceived(SC_ClanWarInfo warInfoMessage)
    {
        currentWarInfo = warInfoMessage;
    }
}