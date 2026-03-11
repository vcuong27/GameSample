using Unity.VisualScripting;
using UnityEngine;
using System;


[Serializable]
public class PlayerProfileData
{
    public int playerID;
    public string playerName;
    public int level;
    public int experience;
    public int coins;
    public FarmData[] farmDatas;
}


public class PlayerProfile : MonoBehaviour
{

    public static Action OnProfileUpdated;


    private static PlayerProfile _instance;
    public static PlayerProfile Instance => _instance;

    private void Awake()
    {
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    private bool IsInitialized = false;
    private PlayerProfileData CurentProfile;

    public void RequestPlayerProfile()
    {
        OnlineManager.Instance.GetPlayerProfile();
    }

    public void Initialize(PlayerProfileData profileData)
    {
        if (profileData != null)
        {
            CurentProfile = profileData;
        }
        else
        {
            CurentProfile = new PlayerProfileData
            {
                playerID = 1,
                playerName = "Player1",
                level = 1,
                experience = 0,
                coins = 1000
            };
        }

        OnProfileUpdated.Invoke();
        IsInitialized = true;
    }

    public bool IsInitialize()
    {
        return IsInitialized;
    }

    public string GetPlayerName()
    {
        return CurentProfile.playerName;
    }

    public int GetLevel()
    {
        return CurentProfile.level;
    }

    public int GetCoins()
    {
        return CurentProfile.coins;
    }

    public int GetPlayeID()
    {
        return CurentProfile.playerID;
    }

    public void UpdateFarmData(FarmData farmData)
    {
        foreach (FarmData data in CurentProfile.farmDatas)
        {
            if (data.id == farmData.id)
            {
                data.Level = farmData.Level;
                data.CoinPerSecond = farmData.CoinPerSecond;
                data.MaxCoin = farmData.MaxCoin;
                data.CurrentCoin = farmData.CurrentCoin;
                data.StartTime = farmData.StartTime;
                break;
            }
        }
    }
}
